using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Containers.Components.World.SubContext.Initiative
{
    public class InitiativePhase
    {
        public List<GameUnit> Units { get; }
        public GameUnit CurrentActiveUnit { get; private set; }
        public Team CurrentActiveTeam { get; private set; }
        private Team FirstPlayer { get; }
        private int redTeamGold;
        private int blueTeamGold;

        private IRenderable SolBanner { get; }
        private IRenderable LunaBanner { get; }
        private IRenderable CreepBanner { get; }
        private IRenderable NewRoundBanner { get; }

        public InitiativePhase(List<GameUnit> unitList, Team firstTurn)
        {
            CurrentActiveTeam = firstTurn;
            FirstPlayer = firstTurn;
            redTeamGold = 0;
            blueTeamGold = 0;
            Units = unitList;
            const int bannerSize = 500;
            SolBanner = BannerIconProvider.GetBanner(BannerType.SolTurnStart, new Vector2(bannerSize));
            LunaBanner = BannerIconProvider.GetBanner(BannerType.LunaTurnStart, new Vector2(bannerSize));
            CreepBanner = BannerIconProvider.GetBanner(BannerType.CreepTurnStart, new Vector2(bannerSize));
            NewRoundBanner = BannerIconProvider.GetBanner(BannerType.RoundStart, new Vector2(bannerSize));
        }

        #region Team Gold

        private int RedTeamGold
        {
            get => redTeamGold;
            set => redTeamGold = value < 0 ? 0 : value;
        }

        private int BlueTeamGold
        {
            get => blueTeamGold;
            set => blueTeamGold = value < 0 ? 0 : value;
        }

        public int GetGoldForTeam(Team team)
        {
            return team switch
            {
                Team.Blue => blueTeamGold,
                Team.Red => redTeamGold,
                _ => 0
            };
        }

        public void AddGoldToTeam(int goldToAdd, Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    BlueTeamGold += goldToAdd;
                    break;
                case Team.Red:
                    RedTeamGold += goldToAdd;
                    break;
            }
        }

        public void DeductGoldFromTeam(int goldToDeduct, Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    BlueTeamGold -= goldToDeduct;
                    break;
                case Team.Red:
                    RedTeamGold -= goldToDeduct;
                    break;
            }
        }

        #endregion

        #region Controller Actions

        public bool SelectActiveUnit(GameUnit unit)
        {
            if (!Units.Contains(unit) || unit.IsExhausted || CurrentActiveTeam != unit.Team) return false;

            CurrentActiveUnit = unit;
            return true;
        }

        public void SelectNextUnitOnActiveTeam()
        {
            List<GameUnit> teamUnits = Units.FindAll(unit => unit.Team == CurrentActiveTeam && unit.IsActive);

            int currentUnitIndex = teamUnits.FindIndex(unit => unit == CurrentActiveUnit);

            int nextUnitIndex = (currentUnitIndex + 1 < teamUnits.Count) ? currentUnitIndex + 1 : 0;

            CurrentActiveUnit = teamUnits[nextUnitIndex];
        }

        public void SelectPreviousUnitOnActiveTeam()
        {
            List<GameUnit> teamUnits = Units.FindAll(unit => unit.Team == CurrentActiveTeam && unit.IsActive);

            int currentUnitIndex = teamUnits.FindIndex(unit => unit == CurrentActiveUnit);

            int nextUnitIndex = (currentUnitIndex - 1 >= 0) ? currentUnitIndex - 1 : teamUnits.Count - 1;

            CurrentActiveUnit = teamUnits[nextUnitIndex];
        }

        #endregion


        public void PassTurnToNextUnit()
        {
            if (GlobalContext.Scenario.GameIsOver) return;

            Team opposingTeam = OpposingTeam(CurrentActiveTeam);

            CurrentActiveUnit.ExhaustAndDisableUnit();

            if (TeamHasExhaustedAllUnits(Team.Creep))
            {
                if (TeamHasExhaustedAllUnits(opposingTeam))
                {
                    if (TeamHasExhaustedAllUnits(CurrentActiveTeam))
                    {
                        StartNewRound();
                        return;
                    }
                }
                else
                {
                    CurrentActiveTeam = opposingTeam;
                }
            }
            else
            {
                CurrentActiveTeam = Team.Creep;
            }

            StartNewTurn();
        }

        public void StartFirstTurn()
        {
            StartNewRound();

            IEnumerable<CreepUnit> creepUnits = GlobalContext.Units.Where(unit => unit is CreepUnit && unit.IsAlive).Cast<CreepUnit>();
            foreach (CreepUnit creepUnit in creepUnits)
            {
                creepUnit.ReadyNextRoutine();

                //Creeps should not act on the first turn
                creepUnit.ExhaustAndDisableUnit();
            }
        }

        private void StartNewRound()
        {
            CurrentActiveTeam = TeamWithFewerRemainingUnits;
            CurrentActiveUnit = Units.FirstOrDefault(unit => unit.Team == CurrentActiveTeam && unit.IsAlive);
            GlobalContext.WorldContext.ResetCursorToActiveUnit();

            Vector2 cursorMapCoordinates = GlobalContext.MapCursor.MapCoordinates;
            GlobalContext.StatusScreenHUD.UpdateWindows();

            //Events

            GlobalEventQueue.QueueSingleEvent(new CameraCursorPositionEvent(cursorMapCoordinates));
            GlobalEventQueue.QueueSingleEvent(new CenterScreenRenderableEvent(NewRoundBanner, 100));
            GlobalEventQueue.QueueSingleEvent(new ToastAtCoordinatesEvent(
                cursorMapCoordinates,
                "Resolving Status Effects...",
                AssetManager.MenuConfirmSFX,
                100
            ));
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(80));
            Units.ForEach(unit => unit.ActivateUnit());

            GlobalEventQueue.QueueSingleEvent(
                new EffectTilesStartOfRoundEvent(
                    new FirstTurnOfNewRoundEvent(this)
                )
            );
        }

        public void StartFirstTurnOfNewRound()
        {
            CurrentActiveTeam = Units.Any(unit => unit is CreepUnit creep && creep.IsAlive && !creep.IsExhausted)
                ? Team.Creep
                : TeamWithFewerRemainingUnits;

            StartNewTurn();
        }

        private void StartNewTurn()
        {
            Units.Where(unit => unit.Team == CurrentActiveTeam).ToList().ForEach(unit => unit.EnableUnit());
            CurrentActiveUnit =
                Units.FirstOrDefault(unit => unit.Team == CurrentActiveTeam && unit.IsAlive && unit.IsActive);

            IRenderable banner;

            switch (CurrentActiveTeam)
            {
                case Team.Blue:
                    DisableTeam(Team.Red);
                    DisableTeam(Team.Creep);
                    banner = LunaBanner;
                    break;
                case Team.Red:
                    DisableTeam(Team.Blue);
                    DisableTeam(Team.Creep);
                    banner = SolBanner;
                    break;
                case Team.Creep:
                    DisableTeam(Team.Blue);
                    DisableTeam(Team.Red);
                    banner = CreepBanner;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Events

            Vector2 activeUnitCoordinates =
                CurrentActiveUnit?.UnitEntity.MapCoordinates ?? Vector2.Zero;
            GlobalEventQueue.QueueSingleEvent(new CameraCursorPositionEvent(activeUnitCoordinates));
            GlobalEventQueue.QueueSingleEvent(new CenterScreenRenderableEvent(banner,
                CurrentActiveTeam == Team.Creep ? 30 : 60, AssetManager.MenuConfirmSFX));

            if (CurrentActiveTeam != Team.Creep)
            {
                GlobalEventQueue.QueueSingleEvent(
                    new ToastAtCoordinatesEvent(
                        activeUnitCoordinates,
                        "Select a unit!",
                        AssetManager.MenuConfirmSFX,
                        120)
                );
            }


            if (CurrentActiveTeam != Team.Creep || CurrentActiveUnit == null) return;

            if (CurrentActiveUnit is CreepUnit activeCreep)
            {
                activeCreep.ExecuteNextRoutine();
                GlobalEventQueue.QueueSingleEvent(new ReadyAIRoutineEvent(activeCreep));
            }
        }

        private bool TeamHasExhaustedAllUnits(Team team)
        {
            return Units
                .Where(unit => unit.Team == team)
                .ToList()
                .TrueForAll(unit => unit.IsExhausted);
        }

        private Team OpposingTeam(Team activeTeam)
        {
            return activeTeam switch
            {
                Team.Blue => Team.Red,
                Team.Red => Team.Blue,
                _ => TeamWithFewerRemainingUnits
            };
        }

        public Team TeamWithFewerRemainingUnits
        {
            get
            {
                int redTeamUnits = Units.Count(unit => unit.Team == Team.Red && unit.IsAlive);
                int blueTeamUnits = Units.Count(unit => unit.Team == Team.Blue && unit.IsAlive);

                if (redTeamUnits == blueTeamUnits) return FirstPlayer;

                if (redTeamUnits == 0) return Team.Blue;
                if (blueTeamUnits == 0) return Team.Red;

                return (redTeamUnits > blueTeamUnits) ? Team.Blue : Team.Red;
            }
        }

        private void DisableTeam(Team team)
        {
            Units.Where(unit => unit.Team == team).ToList().ForEach(unit => unit.DisableInactiveUnit());
        }
    }
}
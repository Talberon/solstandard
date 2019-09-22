using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Containers.Contexts
{
    public class InitiativeContext
    {
        public List<GameUnit> Units { get; }
        public GameUnit CurrentActiveUnit { get; private set; }
        public Team CurrentActiveTeam { get; private set; }
        private Team FirstPlayer { get; }
        private int redTeamGold;
        private int blueTeamGold;

        public InitiativeContext(List<GameUnit> unitList, Team firstTurn)
        {
            CurrentActiveTeam = firstTurn;
            FirstPlayer = firstTurn;
            redTeamGold = 0;
            blueTeamGold = 0;
            Units = unitList;
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
            switch (team)
            {
                case Team.Blue:
                    return blueTeamGold;
                case Team.Red:
                    return redTeamGold;
                default:
                    return 0;
            }
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
            Team opposingTeam = OpposingTeam(CurrentActiveTeam);

            CurrentActiveUnit.ExhaustAndDisableUnit();

            if (TeamHasExhaustedAllUnits(Team.Creep))
            {
                if (TeamHasExhaustedAllUnits(opposingTeam))
                {
                    if (TeamHasExhaustedAllUnits(CurrentActiveTeam))
                    {
                        StartNewRound();
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

            IEnumerable<CreepUnit> creepUnits = GameContext.Units.Where(unit => unit is CreepUnit).Cast<CreepUnit>();
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
            GameContext.GameMapContext.ResetCursorToActiveUnit();

            Vector2 cursorMapCoordinates = GameContext.MapCursor.MapCoordinates;
            RefreshAllUnits();
            GameContext.StatusScreenView.UpdateWindows();

            //Events

            GlobalEventQueue.QueueSingleEvent(new CameraCursorPositionEvent(cursorMapCoordinates));
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(
                    cursorMapCoordinates,
                    "ROUND " + GameContext.GameMapContext.RoundCounter + " STARTING...",
                    100
                )
            );
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(80));
            GlobalEventQueue.QueueSingleEvent(new EffectTilesStartOfRoundEvent());
            GlobalEventQueue.QueueSingleEvent(new FirstTurnOfNewRoundEvent(this));
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

            switch (CurrentActiveTeam)
            {
                case Team.Blue:
                    DisableTeam(Team.Red);
                    DisableTeam(Team.Creep);
                    break;
                case Team.Red:
                    DisableTeam(Team.Blue);
                    DisableTeam(Team.Creep);
                    break;
                case Team.Creep:
                    DisableTeam(Team.Blue);
                    DisableTeam(Team.Red);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Events

            string playerInstruction = (CurrentActiveTeam != Team.Creep)
                ? Environment.NewLine + "Select a unit."
                : string.Empty;

            Vector2 activeUnitCoordinates =
                CurrentActiveUnit?.UnitEntity.MapCoordinates ?? Vector2.Zero;
            GlobalEventQueue.QueueSingleEvent(new CameraCursorPositionEvent(activeUnitCoordinates));
            GlobalEventQueue.QueueSingleEvent(
                new ToastAtCoordinatesEvent(
                    activeUnitCoordinates,
                    $"{CurrentActiveTeam} Turn START!{playerInstruction}",
                    AssetManager.MenuConfirmSFX,
                    120
                )
            );

            if (CurrentActiveTeam != Team.Creep || CurrentActiveUnit == null) return;

            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));

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
            switch (activeTeam)
            {
                case Team.Blue:
                    return Team.Red;
                case Team.Red:
                    return Team.Blue;
                default:
                    return FirstPlayer;
            }
        }

        private void RefreshAllUnits()
        {
            Units.ForEach(unit => unit.ActivateUnit());

            GlobalEventQueue.QueueSingleEvent(new ToastAtCursorEvent(
                "Refreshing units...",
                AssetManager.MenuConfirmSFX,
                100
            ));
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
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
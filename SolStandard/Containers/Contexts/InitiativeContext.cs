using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Containers.Contexts
{
    public enum TurnOrder
    {
        AlternateExhaustingUnits
    }

    public class InitiativeContext
    {
        public List<GameUnit> InitiativeList { get; }
        public GameUnit CurrentActiveUnit { get; private set; }
        public Team CurrentActiveTeam { get; private set; }
        private Team FirstPlayer { get; }

        public InitiativeContext(List<GameUnit> unitList, Team firstTurn,
            TurnOrder turnOrder = TurnOrder.AlternateExhaustingUnits)
        {
            CurrentActiveTeam = firstTurn;
            FirstPlayer = firstTurn;

            switch (turnOrder)
            {
                case TurnOrder.AlternateExhaustingUnits:
                    InitiativeList = TeamByTeamTurnOrder(unitList, firstTurn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(turnOrder), turnOrder, null);
            }
        }

        public void StartFirstTurn()
        {
            CurrentActiveTeam = TeamWithFewerRemainingUnits();
            CurrentActiveUnit = InitiativeList.FirstOrDefault(unit => unit.Team == CurrentActiveTeam && unit.IsAlive);
            
            foreach (CreepUnit creepUnit in GameContext.Units.Where(unit=>unit is CreepUnit).Cast<CreepUnit>())
            {
                creepUnit.ReadyNextRoutine();
            }
            
            StartNewRound();
        }

        public void SelectNextUnitOnActiveTeam()
        {
            List<GameUnit> teamUnits = InitiativeList.FindAll(unit => unit.Team == CurrentActiveTeam && unit.IsActive);

            int currentUnitIndex = teamUnits.FindIndex(unit => unit == CurrentActiveUnit);

            int nextUnitIndex = (currentUnitIndex + 1 < teamUnits.Count) ? currentUnitIndex + 1 : 0;

            CurrentActiveUnit = teamUnits[nextUnitIndex];
        }

        public void SelectPreviousUnitOnActiveTeam()
        {
            List<GameUnit> teamUnits = InitiativeList.FindAll(unit => unit.Team == CurrentActiveTeam && unit.IsActive);

            int currentUnitIndex = teamUnits.FindIndex(unit => unit == CurrentActiveUnit);

            int nextUnitIndex = (currentUnitIndex - 1 >= 0) ? currentUnitIndex - 1 : teamUnits.Count - 1;

            CurrentActiveUnit = teamUnits[nextUnitIndex];
        }

        public void PassTurnToNextUnit()
        {
            Team opposingTeam = OpposingTeam;

            CurrentActiveUnit.ExhaustAndDisableUnit();

            if (TeamHasExhaustedAllUnits(opposingTeam))
            {
                if (TeamHasExhaustedAllUnits(CurrentActiveTeam))
                {
                    if (TeamHasExhaustedAllUnits(Team.Creep))
                    {
                        StartNewRound();
                    }
                    else
                    {
                        CurrentActiveTeam = Team.Creep;
                    }
                }
            }
            else
            {
                CurrentActiveTeam = opposingTeam;
            }

            UpdateUnitActivation();
        }

        private void StartNewRound()
        {
            CurrentActiveTeam = TeamWithFewerRemainingUnits();
            CurrentActiveUnit = InitiativeList.FirstOrDefault(unit => unit.Team == CurrentActiveTeam && unit.IsAlive);
            GameContext.GameMapContext.ResetCursorToActiveUnit();

            Queue<IEvent> newRoundEvents = new Queue<IEvent>();
            Vector2 cursorMapCoordinates = GameContext.MapCursor.MapCoordinates;
            newRoundEvents.Enqueue(new CameraCursorPositionEvent(cursorMapCoordinates));
            newRoundEvents.Enqueue(
                new ToastAtCoordinatesEvent(
                    cursorMapCoordinates,
                    "ROUND " + GameContext.GameMapContext.RoundCounter + " STARTING...",
                    100
                )
            );
            newRoundEvents.Enqueue(new WaitFramesEvent(80));
            GlobalEventQueue.QueueEvents(newRoundEvents);

            RefreshAllUnits();
            GlobalEventQueue.QueueSingleEvent(new EffectTilesStartOfRoundEvent());

            GlobalEventQueue.QueueSingleEvent(new UpdateTurnOrderEvent(this));
            
            GameContext.StatusScreenView.UpdateWindows();
        }

        public void UpdateTurnOrder()
        {
            CurrentActiveTeam = TeamWithFewerRemainingUnits();
            UpdateUnitActivation();
        }

        private Team TeamWithFewerRemainingUnits()
        {
            int redTeamUnits = InitiativeList.Count(unit => unit.Team == Team.Red && unit.IsAlive);
            int blueTeamUnits = InitiativeList.Count(unit => unit.Team == Team.Blue && unit.IsAlive);

            if (redTeamUnits == blueTeamUnits) return FirstPlayer;

            if (redTeamUnits == 0) return Team.Blue;
            if (blueTeamUnits == 0) return Team.Red;

            return (redTeamUnits > blueTeamUnits) ? Team.Blue : Team.Red;
        }

        private void UpdateUnitActivation()
        {
            InitiativeList.Where(unit => unit.Team == CurrentActiveTeam).ToList().ForEach(unit => unit.EnableUnit());
            CurrentActiveUnit =
                InitiativeList.FirstOrDefault(unit => unit.Team == CurrentActiveTeam && unit.IsAlive && unit.IsActive);


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

            string playerInstruction = (CurrentActiveTeam != Team.Creep)
                ? Environment.NewLine + "Select a unit."
                : string.Empty;

            Queue<IEvent> activationEvents = new Queue<IEvent>();
            Vector2 activeUnitCoordinates =
                CurrentActiveUnit?.UnitEntity.MapCoordinates ?? Vector2.Zero;
            activationEvents.Enqueue(new CameraCursorPositionEvent(activeUnitCoordinates));
            activationEvents.Enqueue(
                new ToastAtCoordinatesEvent(
                    activeUnitCoordinates,
                    $"{CurrentActiveTeam} Turn START! {playerInstruction}",
                    AssetManager.MenuConfirmSFX,
                    120
                )
            );
            GlobalEventQueue.QueueEvents(activationEvents);

            ExecuteCreepRoutine();
        }

        private void ExecuteCreepRoutine()
        {
            if (CurrentActiveTeam != Team.Creep) return;
            if (GameContext.ActiveUnit.UnitEntity == null) return;
            
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));

            if (GameContext.ActiveUnit is CreepUnit activeCreep)
            {
                activeCreep.ExecuteNextRoutine();
                GlobalEventQueue.QueueSingleEvent(new ReadyAIRoutineEvent(activeCreep));
            }
        }

        private void DisableTeam(Team team)
        {
            InitiativeList.Where(unit => unit.Team == team).ToList().ForEach(unit => unit.DisableInactiveUnit());
        }

        private bool TeamHasExhaustedAllUnits(Team team)
        {
            return InitiativeList
                .Where(unit => unit.Team == team)
                .ToList()
                .TrueForAll(unit => unit.IsExhausted);
        }

        private Team OpposingTeam
        {
            get
            {
                Team opposingTeam;

                switch (CurrentActiveTeam)
                {
                    case Team.Blue:
                        opposingTeam = Team.Red;
                        break;
                    case Team.Red:
                        opposingTeam = Team.Blue;
                        break;
                    default:
                        opposingTeam = Team.Creep;
                        break;
                }

                return opposingTeam;
            }
        }

        private void RefreshAllUnits()
        {
            Queue<IEvent> refreshUnitEvents = new Queue<IEvent>();
            refreshUnitEvents.Enqueue(
                new ToastAtCursorEvent(
                    "Refreshing units...",
                    AssetManager.MenuConfirmSFX,
                    100
                )
            );
            refreshUnitEvents.Enqueue(new WaitFramesEvent(50));
            GlobalEventQueue.QueueEvents(refreshUnitEvents);

            InitiativeList.ForEach(unit => unit.ActivateUnit());
        }

        public bool SelectActiveUnit(GameUnit unit)
        {
            if (!InitiativeList.Contains(unit) || unit.IsExhausted || CurrentActiveTeam != unit.Team) return false;

            CurrentActiveUnit = unit;
            return true;
        }

        private static List<GameUnit> TeamByTeamTurnOrder(IEnumerable<GameUnit> unitList, Team firstTurn)
        {
            //Split the initiative list into each team
            List<GameUnit> redTeam = new List<GameUnit>();
            List<GameUnit> blueTeam = new List<GameUnit>();
            List<GameUnit> creepTeam = new List<GameUnit>();

            unitList = unitList.OrderBy(unit => !unit.IsCommander);
            PopulateTeams(unitList, redTeam, blueTeam, creepTeam);

            List<GameUnit> newInitiativeList = new List<GameUnit>();

            switch (firstTurn)
            {
                case Team.Red:
                    newInitiativeList.AddRange(redTeam);
                    newInitiativeList.AddRange(blueTeam);
                    newInitiativeList.AddRange(creepTeam);
                    break;
                case Team.Blue:
                    newInitiativeList.AddRange(blueTeam);
                    newInitiativeList.AddRange(redTeam);
                    newInitiativeList.AddRange(creepTeam);
                    break;
                case Team.Creep:
                    newInitiativeList.AddRange(creepTeam);
                    newInitiativeList.AddRange(redTeam);
                    newInitiativeList.AddRange(blueTeam);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(firstTurn), firstTurn, null);
            }

            return newInitiativeList;
        }

        private static void PopulateTeams(IEnumerable<GameUnit> unitList, List<GameUnit> redTeam,
            List<GameUnit> blueTeam, List<GameUnit> creepTeam)
        {
            foreach (GameUnit unit in unitList)
            {
                switch (unit.Team)
                {
                    case Team.Red:
                        redTeam.Add(unit);
                        break;
                    case Team.Blue:
                        blueTeam.Add(unit);
                        break;
                    case Team.Creep:
                        creepTeam.Add(unit);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Containers.Contexts
{
    public enum TurnOrder
    {
        TeamByTeam,
        UnitByUnit,
        AlternateExhaustingUnits
    }

    public class InitiativeContext
    {
        public List<GameUnit> InitiativeList { get; private set; }
        public GameUnit CurrentActiveUnit { get; private set; }
        public Team CurrentActiveTeam { get; private set; }
        private Team FirstPlayer { get; set; }

        public InitiativeContext(IEnumerable<GameUnit> unitList, Team firstTurn,
            TurnOrder turnOrder = TurnOrder.UnitByUnit)
        {
            CurrentActiveTeam = firstTurn;
            FirstPlayer = firstTurn;

            switch (turnOrder)
            {
                case TurnOrder.UnitByUnit:
                    InitiativeList = UnitByUnitTurnOrder(unitList, firstTurn);
                    break;
                case TurnOrder.TeamByTeam:
                    InitiativeList = TeamByTeamTurnOrder(unitList, firstTurn);
                    break;
                case TurnOrder.AlternateExhaustingUnits:
                    InitiativeList = TeamByTeamTurnOrder(unitList, firstTurn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("turnOrder", turnOrder, null);
            }
        }

        public void StartFirstTurn()
        {
            StartNewRound();
        }


        public void PassTurnToNextUnit()
        {
            Team opposingTeam = OpposingTeam;

            CurrentActiveUnit.DisableExhaustedUnit();

            if (TeamHasExhaustedAllUnits(opposingTeam))
            {
                if (TeamHasExhaustedAllUnits(CurrentActiveTeam))
                {
                    if (TeamHasExhaustedAllUnits(Team.Creep))
                    {
                        RefreshAllUnits();
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

        private void UpdateUnitActivation()
        {
            InitiativeList.Where(unit => unit.Team == CurrentActiveTeam).ToList().ForEach(unit => unit.EnableUnit());
            CurrentActiveUnit =
                InitiativeList.First(unit => unit.Team == CurrentActiveTeam && unit.IsAlive && !unit.IsExhausted);


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

            Queue<IEvent> activationEvents = new Queue<IEvent>();
            Vector2 activeUnitCoordinates = CurrentActiveUnit.UnitEntity.MapCoordinates;
            activationEvents.Enqueue(new CameraCursorPositionEvent(activeUnitCoordinates));
            activationEvents.Enqueue(
                new ToastAtCoordinatesEvent(activeUnitCoordinates, string.Format("{0} Turn Start!", CurrentActiveTeam))
            );
            GlobalEventQueue.QueueEvents(activationEvents);
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

        private void StartNewRound()
        {
            CurrentActiveTeam = FirstPlayer;
            RefreshAllUnits();
            DisableTeam(OpposingTeam);
            DisableTeam(Team.Creep);
            CurrentActiveUnit = InitiativeList.First(unit => unit.Team == CurrentActiveTeam);

            Queue<IEvent> newRoundEvents = new Queue<IEvent>();
            Vector2 activeUnitCoordinates = CurrentActiveUnit.UnitEntity.MapCoordinates;
            newRoundEvents.Enqueue(new CameraCursorPositionEvent(activeUnitCoordinates));
            newRoundEvents.Enqueue(new ToastAtCoordinatesEvent(activeUnitCoordinates, "New Round!"));
            newRoundEvents.Enqueue(new WaitFramesEvent(50));
            GlobalEventQueue.QueueEvents(newRoundEvents);
        }

        private void RefreshAllUnits()
        {
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

            PopulateAndShuffleTeams(unitList, redTeam, blueTeam, creepTeam);

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
                    throw new ArgumentOutOfRangeException("firstTurn", firstTurn, null);
            }

            return newInitiativeList;
        }

        /// <summary>
        /// Dynamic initiative sequencing depending on size of armies
        /// If both armies have the same number of units, go in alternating order A B A B A B A B
        /// If one army has twice as many units as another, go in 122 sequence A B B A B B A B B A
        /// Likewise if one army has x3 units as another, go int 1222 sequence A B B B A B B B A
        ///
        /// If an army has more units than another but isn't a multiple of the other army's size,
        /// go in the appropriate lower algorithm, with the team that has extra units going last.
        ///
        /// Examples:
        /// 3 v 5: A B A B A B B B
        /// 3 v 8: A B B A B B A B B B B
        /// 3 v 11: A B B B A B B B A B B B B B
        /// </summary>
        /// <param name="unitList"></param>
        /// <param name="firstTurnIfTie"></param>
        /// <returns>A list of units sorted in the determined turn order</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static List<GameUnit> UnitByUnitTurnOrder(IEnumerable<GameUnit> unitList, Team firstTurnIfTie)
        {
            //Split the initiative list into each team
            List<GameUnit> redTeam = new List<GameUnit>();
            List<GameUnit> blueTeam = new List<GameUnit>();
            List<GameUnit> creepTeam = new List<GameUnit>();

            PopulateAndShuffleTeams(unitList, redTeam, blueTeam, creepTeam);

            //Determine which team has more units and by how much 
            Queue<GameUnit> minorityTeam;
            Queue<GameUnit> majorityTeam;
            int majorityTurnsForEveryMinorityTurn;

            if (redTeam.Count > blueTeam.Count)
            {
                minorityTeam = new Queue<GameUnit>(blueTeam);
                majorityTeam = new Queue<GameUnit>(redTeam);
                majorityTurnsForEveryMinorityTurn = FindLowestMultiplier(redTeam.Count, blueTeam.Count);
            }
            else if (redTeam.Count < blueTeam.Count)
            {
                minorityTeam = new Queue<GameUnit>(redTeam);
                majorityTeam = new Queue<GameUnit>(blueTeam);
                majorityTurnsForEveryMinorityTurn = FindLowestMultiplier(blueTeam.Count, redTeam.Count);
            }
            else
            {
                //If an override is provided, then that team should go first in the order if they tie
                switch (firstTurnIfTie)
                {
                    case Team.Red:
                        majorityTeam = new Queue<GameUnit>(redTeam);
                        minorityTeam = new Queue<GameUnit>(blueTeam);
                        break;
                    case Team.Blue:
                        majorityTeam = new Queue<GameUnit>(blueTeam);
                        minorityTeam = new Queue<GameUnit>(redTeam);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("firstTurnIfTie", firstTurnIfTie, null);
                }

                majorityTurnsForEveryMinorityTurn = FindLowestMultiplier(majorityTeam.Count, minorityTeam.Count);
            }

            //Build the final initiative list
            List<GameUnit> newInitiativeList = new List<GameUnit>();
            for (int i = 1; i < (redTeam.Count + blueTeam.Count) + 1; i++)
            {
                //Majority has priority
                if (minorityTeam.Count < 1)
                {
                    newInitiativeList.Add(majorityTeam.Dequeue());
                }
                else if (i % (majorityTurnsForEveryMinorityTurn + 1) == 0)
                {
                    //Add minority team
                    newInitiativeList.Add(minorityTeam.Dequeue());
                }
                else
                {
                    //Add majority team
                    newInitiativeList.Add(majorityTeam.Dequeue());
                }
            }

            //Fill the end of the list with Creeps
            newInitiativeList.AddRange(creepTeam);

            return newInitiativeList;
        }

        private static void PopulateAndShuffleTeams(IEnumerable<GameUnit> unitList, List<GameUnit> redTeam,
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

            //Randomize each team's units
            redTeam.Shuffle();
            blueTeam.Shuffle();
            creepTeam.Shuffle();
        }

        private static int FindLowestMultiplier(int value, int factor)
        {
            return (int) (value / (double) factor);
        }
    }
}
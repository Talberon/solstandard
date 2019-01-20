using System;
using System.Collections.Generic;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts
{
    public enum TurnOrder
    {
        TeamByTeam,
        UnitByUnit
    }

    public class InitiativeContext
    {
        public List<GameUnit> InitiativeList { get; private set; }
        public GameUnit CurrentActiveUnit { get; private set; }


        public InitiativeContext(IEnumerable<GameUnit> unitList, Team firstTurn,
            TurnOrder turnOrder = TurnOrder.UnitByUnit)
        {
            switch (turnOrder)
            {
                case TurnOrder.UnitByUnit:
                    InitiativeList = UnitByUnitTurnOrder(unitList, firstTurn);
                    break;
                case TurnOrder.TeamByTeam:
                    InitiativeList = TeamByTeamTurnOrder(unitList, firstTurn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("turnOrder", turnOrder, null);
            }

            CurrentActiveUnit = InitiativeList[0];
        }

        public void PassTurnToNextUnit()
        {
            SetNextUnitInList();
            RotateUnitList();
        }

        private void SetNextUnitInList()
        {
            int currentIndex = InitiativeList.FindIndex(unit => unit == CurrentActiveUnit);

            if (currentIndex < InitiativeList.Count)
            {
                CurrentActiveUnit = InitiativeList[currentIndex + 1];
            }
            else
            {
                CurrentActiveUnit = InitiativeList[0];
            }
        }

        private void RotateUnitList()
        {
            GameUnit firstUnit = InitiativeList[0];
            InitiativeList.RemoveAt(0);
            InitiativeList.Add(firstUnit);
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
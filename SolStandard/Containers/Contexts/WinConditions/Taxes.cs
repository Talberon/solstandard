using System.Collections.Generic;
using System.Linq;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class Taxes : Objective
    {
        public int TargetGold { get; private set; }

        public Taxes(int targetGold) : base("GOLD TARGET REACHED")
        {
            TargetGold = targetGold;
        }

        public override bool ConditionsMet(GameContext gameContext)
        {
            if (TeamHasCollectedTargetGold(Team.Blue) && TeamHasCollectedTargetGold(Team.Red))
            {
                GameIsADraw = true;
                return GameIsADraw;
            }

            if (TeamHasCollectedTargetGold(Team.Blue))
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            if (TeamHasCollectedTargetGold(Team.Red))
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            return false;
        }

        public static int CollectedGold(Team team)
        {
            List<GameUnit> teamUnitList = GameContext.Units.FindAll(unit => unit.Team == team);

            return teamUnitList.Sum(unit => unit.CurrentGold);
        }

        private bool TeamHasCollectedTargetGold(Team team)
        {
            return CollectedGold(team) >= TargetGold;
        }
    }
}
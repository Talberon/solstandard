using System.Collections.Generic;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public class DefeatCommander : WinCondition
    {
        public override bool ConditionsMet(GameContext gameContext)
        {
            List<GameUnit> blueTeam = GameContext.Units.FindAll(unit => unit.Team == Team.Blue);
            List<GameUnit> redTeam = GameContext.Units.FindAll(unit => unit.Team == Team.Red);

            if (TeamMonarchsAreAllDead(blueTeam) && TeamMonarchsAreAllDead(redTeam))
            {
                BothTeamsLose = true;
                return BothTeamsLose;
            }

            if (TeamMonarchsAreAllDead(blueTeam))
            {
                BlueTeamWins = true;
                return BlueTeamWins;
            }

            if (TeamMonarchsAreAllDead(redTeam))
            {
                RedTeamWins = true;
                return RedTeamWins;
            }

            return false;
        }


        private static bool TeamMonarchsAreAllDead(List<GameUnit> team)
        {
            List<GameUnit> teamMonarchs = team.FindAll(unit => unit.Role == Role.Monarch);

            foreach (GameUnit monarch in teamMonarchs)
            {
                //Return false if any Monarchs are alive.
                if (monarch.Stats.Hp > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
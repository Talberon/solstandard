using System.Collections.Generic;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers.Contexts
{
    public static class GameScenario
    {
        public static void CheckForWinState(GameContext gameContext)
        {
            List<GameUnit> blueTeam = GameContext.Units.FindAll(unit => unit.UnitTeam == Team.Blue);
            List<GameUnit> redTeam = GameContext.Units.FindAll(unit => unit.UnitTeam == Team.Red);

            if (TeamMonarchsAreAllDead(blueTeam))
            {
                //TODO Red Team Wins!
                string winText = "RED TEAM WINS!";
                winText += "\nRED TEAM WINS!";
                winText += "\nRED TEAM WINS!";
                winText += "\nRED TEAM WINS!";
                winText += "\nRED TEAM WINS!";
                winText += "\nRED TEAM WINS!";
                winText += "\nRED TEAM WINS!";
                gameContext.MapContext.HelpText = winText;
            }

            if (TeamMonarchsAreAllDead(redTeam))
            {
                //TODO Blue Team Wins!
                string winText = "BLUE TEAM WINS!";
                winText += "\nBLUE TEAM WINS!";
                winText += "\nBLUE TEAM WINS!";
                winText += "\nBLUE TEAM WINS!";
                winText += "\nBLUE TEAM WINS!";
                winText += "\nBLUE TEAM WINS!";
                winText += "\nBLUE TEAM WINS!";
                gameContext.MapContext.HelpText = winText;
            }
        }

        private static bool TeamMonarchsAreAllDead(List<GameUnit> team)
        {
            List<GameUnit> teamMonarchs = team.FindAll(unit => unit.UnitJobClass == UnitClass.Monarch);

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
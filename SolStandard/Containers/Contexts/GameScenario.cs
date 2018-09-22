using System.Collections.Generic;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts
{
    public static class GameScenario
    {
        public static void CheckForWinState(GameContext gameContext)
        {
            List<GameUnit> blueTeam = GameContext.Units.FindAll(unit => unit.Team == Team.Blue);
            List<GameUnit> redTeam = GameContext.Units.FindAll(unit => unit.Team == Team.Red);

            if (TeamMonarchsAreAllDead(blueTeam))
            {
                gameContext.ResultsUI.RedTeamResultText = "RED TEAM WINS!";
                gameContext.ResultsUI.BlueTeamResultText = "BLUE TEAM IS DEFEATED...";
                GameContext.CurrentGameState = GameContext.GameState.Results;
                MusicBox.Play(AssetManager.MusicTracks.Find(song => song.Name.Equals("VictoryTheme")),0.5f);
            }

            if (TeamMonarchsAreAllDead(redTeam))
            {
                gameContext.ResultsUI.BlueTeamResultText = "BLUE TEAM WINS!";
                gameContext.ResultsUI.RedTeamResultText = "RED TEAM IS DEFEATED...";
                GameContext.CurrentGameState = GameContext.GameState.Results;
                MusicBox.Play(AssetManager.MusicTracks.Find(song => song.Name.Equals("VictoryTheme")),0.5f);
            }

            if (TeamMonarchsAreAllDead(blueTeam) && TeamMonarchsAreAllDead(redTeam))
            {
                gameContext.ResultsUI.BlueTeamResultText = "BLUE TEAM IS DEFEATED...";
                gameContext.ResultsUI.RedTeamResultText = "RED TEAM IS DEFEATED...";
                GameContext.CurrentGameState = GameContext.GameState.Results;
                MusicBox.Play(AssetManager.MusicTracks.Find(song => song.Name.Equals("VictoryTheme")),0.5f);
            }
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
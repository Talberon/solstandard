using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public abstract class WinCondition
    {
        protected bool BlueTeamWins;
        protected bool RedTeamWins;
        protected bool BothTeamsLose;

        public abstract bool ConditionsMet(GameContext gameContext);

        public void EndGame(GameContext gameContext)
        {
            if (RedTeamWins)
            {
                gameContext.StatusUI.RedTeamResultText = "RED TEAM WINS!";
                gameContext.StatusUI.BlueTeamResultText = "BLUE TEAM IS DEFEATED...";
                GameContext.CurrentGameState = GameContext.GameState.Results;
                MusicBox.Play(AssetManager.MusicTracks.Find(song => song.Name.Equals("VictoryTheme")), 0.5f);
            }

            if (BlueTeamWins)
            {
                gameContext.StatusUI.BlueTeamResultText = "BLUE TEAM WINS!";
                gameContext.StatusUI.RedTeamResultText = "RED TEAM IS DEFEATED...";
                GameContext.CurrentGameState = GameContext.GameState.Results;
                MusicBox.Play(AssetManager.MusicTracks.Find(song => song.Name.Equals("VictoryTheme")), 0.5f);
            }

            if (BothTeamsLose)
            {
                gameContext.StatusUI.BlueTeamResultText = "BLUE TEAM IS DEFEATED...";
                gameContext.StatusUI.RedTeamResultText = "RED TEAM IS DEFEATED...";
                GameContext.CurrentGameState = GameContext.GameState.Results;
                MusicBox.Play(AssetManager.MusicTracks.Find(song => song.Name.Equals("VictoryTheme")), 0.5f);
            }
        }
    }
}
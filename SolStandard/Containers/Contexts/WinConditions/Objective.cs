using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public abstract class Objective
    {
        public string VictoryLabelText { get; private set; }

        protected bool BlueTeamWins;
        protected bool RedTeamWins;
        protected bool GameIsADraw;

        protected Objective(string victoryLabelText)
        {
            VictoryLabelText = victoryLabelText;
        }

        public abstract bool ConditionsMet(GameContext gameContext);

        public void EndGame(GameContext gameContext)
        {
            gameContext.StatusUI.ResultLabelText = VictoryLabelText;

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

            if (GameIsADraw)
            {
                gameContext.StatusUI.BlueTeamResultText = "DRAW...";
                gameContext.StatusUI.RedTeamResultText = "DRAW...";
                GameContext.CurrentGameState = GameContext.GameState.Results;
                MusicBox.Play(AssetManager.MusicTracks.Find(song => song.Name.Equals("VictoryTheme")), 0.5f);
            }
        }
    }
}
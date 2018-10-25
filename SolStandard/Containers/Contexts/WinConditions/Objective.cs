using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public enum VictoryConditions
    {
        Surrender,
        Assassinate,
        Taxes,
        LastMan,
        Seize
    }
    
    public abstract class Objective
    {
        protected abstract IRenderable VictoryLabelContent { get; }
        protected static Color ObjectiveWindowColor = new Color(60, 60, 60, 180);
        protected bool BlueTeamWins;
        protected bool RedTeamWins;
        protected bool GameIsADraw;

        public virtual IRenderable ObjectiveInfo
        {
            get { return new RenderBlank(); }
        }

        public abstract bool ConditionsMet(GameContext gameContext);

        public void EndGame(GameContext gameContext)
        {
            gameContext.StatusScreenView.ResultLabelContent = VictoryLabelContent;

            if (RedTeamWins)
            {
                gameContext.StatusScreenView.RedTeamResultText = "RED TEAM WINS!";
                gameContext.StatusScreenView.BlueTeamResultText = "BLUE TEAM IS DEFEATED...";
                TransferToResultsScreen();
            }

            if (BlueTeamWins)
            {
                gameContext.StatusScreenView.BlueTeamResultText = "BLUE TEAM WINS!";
                gameContext.StatusScreenView.RedTeamResultText = "RED TEAM IS DEFEATED...";
                TransferToResultsScreen();
            }

            if (GameIsADraw)
            {
                gameContext.StatusScreenView.BlueTeamResultText = "DRAW...";
                gameContext.StatusScreenView.RedTeamResultText = "DRAW...";
                TransferToResultsScreen();
            }
        }

        private static void TransferToResultsScreen()
        {
            GameContext.CurrentGameState = GameContext.GameState.Results;
            MusicBox.PlayLoop(AssetManager.MusicTracks.Find(song => song.Name.Equals("VictoryTheme")), 0.5f);
        }
    }
}
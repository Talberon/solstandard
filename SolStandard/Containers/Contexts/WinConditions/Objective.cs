using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts.WinConditions
{
    public enum VictoryConditions
    {
        Assassinate,
        RoutArmy,
        Seize,
        Taxes,
        Surrender,
        Escape,
        SoloDefeatBoss,
        CollectTheRelicsVS,
        CollectTheRelicsCoOp
    }

    public abstract class Objective
    {
        protected abstract IRenderable VictoryLabelContent { get; }
        protected static Color ObjectiveWindowColor = new Color(30, 30, 30, 180);
        protected bool BlueTeamWins;
        protected bool RedTeamWins;
        protected bool GameIsADraw;
        protected bool CoOpVictory;
        protected bool AllPlayersLose;

        public virtual IRenderable ObjectiveInfo
        {
            get { return new RenderBlank(); }
        }

        public abstract bool ConditionsMet();

        public void EndGame()
        {
            GameContext.StatusScreenView.ResultLabelContent = VictoryLabelContent;

            if (RedTeamWins)
            {
                GameContext.StatusScreenView.RedTeamResultText = "RED TEAM WINS!";
                GameContext.StatusScreenView.BlueTeamResultText = "BLUE TEAM IS DEFEATED...";
                TransferToResultsScreen();
            }

            if (BlueTeamWins)
            {
                GameContext.StatusScreenView.BlueTeamResultText = "BLUE TEAM WINS!";
                GameContext.StatusScreenView.RedTeamResultText = "RED TEAM IS DEFEATED...";
                TransferToResultsScreen();
            }

            if (GameIsADraw)
            {
                GameContext.StatusScreenView.BlueTeamResultText = "DRAW...";
                GameContext.StatusScreenView.RedTeamResultText = "DRAW...";
                TransferToResultsScreen();
            }

            if (CoOpVictory)
            {
                GameContext.StatusScreenView.BlueTeamResultText = "CO-OP VICTORY!";
                GameContext.StatusScreenView.RedTeamResultText = "CO-OP VICTORY!";
                TransferToResultsScreen();
            }

            if (AllPlayersLose)
            {
                GameContext.StatusScreenView.BlueTeamResultText = "YOU LOSE...";
                GameContext.StatusScreenView.RedTeamResultText = "YOU LOSE...";
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
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Scenario
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

        public virtual IRenderable ObjectiveInfo => RenderBlank.Blank;

        public abstract bool ConditionsMet { get; }

        public void EndGame()
        {
            GlobalContext.StatusScreenView.ResultLabelContent = VictoryLabelContent;

            if (RedTeamWins)
            {
                GlobalContext.StatusScreenView.RedTeamResultText = "RED TEAM WINS!";
                GlobalContext.StatusScreenView.BlueTeamResultText = "BLUE TEAM IS DEFEATED...";
                TransferToResultsScreen();
            }

            if (BlueTeamWins)
            {
                GlobalContext.StatusScreenView.BlueTeamResultText = "BLUE TEAM WINS!";
                GlobalContext.StatusScreenView.RedTeamResultText = "RED TEAM IS DEFEATED...";
                TransferToResultsScreen();
            }

            if (GameIsADraw)
            {
                GlobalContext.StatusScreenView.BlueTeamResultText = "DRAW...";
                GlobalContext.StatusScreenView.RedTeamResultText = "DRAW...";
                TransferToResultsScreen();
            }

            if (CoOpVictory)
            {
                GlobalContext.StatusScreenView.BlueTeamResultText = "CO-OP VICTORY!";
                GlobalContext.StatusScreenView.RedTeamResultText = "CO-OP VICTORY!";
                TransferToResultsScreen();
            }

            if (AllPlayersLose)
            {
                GlobalContext.StatusScreenView.BlueTeamResultText = "YOU LOSE...";
                GlobalContext.StatusScreenView.RedTeamResultText = "YOU LOSE...";
                TransferToResultsScreen();
            }
        }

        private static void TransferToResultsScreen()
        {
            GlobalContext.CurrentGameState = GlobalContext.GameState.Results;
            MusicBox.Play(AssetManager.MusicTracks.Find(song => song.Name.EndsWith("VictoryJingle")));
        }
    }
}
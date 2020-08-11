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
            GlobalContext.StatusScreenHUD.ResultLabelContent = VictoryLabelContent;

            if (RedTeamWins)
            {
                GlobalContext.StatusScreenHUD.RedTeamResultText = "RED TEAM WINS!";
                GlobalContext.StatusScreenHUD.BlueTeamResultText = "BLUE TEAM IS DEFEATED...";
                TransferToResultsScreen();
            }

            if (BlueTeamWins)
            {
                GlobalContext.StatusScreenHUD.BlueTeamResultText = "BLUE TEAM WINS!";
                GlobalContext.StatusScreenHUD.RedTeamResultText = "RED TEAM IS DEFEATED...";
                TransferToResultsScreen();
            }

            if (GameIsADraw)
            {
                GlobalContext.StatusScreenHUD.BlueTeamResultText = "DRAW...";
                GlobalContext.StatusScreenHUD.RedTeamResultText = "DRAW...";
                TransferToResultsScreen();
            }

            if (CoOpVictory)
            {
                GlobalContext.StatusScreenHUD.BlueTeamResultText = "CO-OP VICTORY!";
                GlobalContext.StatusScreenHUD.RedTeamResultText = "CO-OP VICTORY!";
                TransferToResultsScreen();
            }

            if (AllPlayersLose)
            {
                GlobalContext.StatusScreenHUD.BlueTeamResultText = "YOU LOSE...";
                GlobalContext.StatusScreenHUD.RedTeamResultText = "YOU LOSE...";
                TransferToResultsScreen();
            }
        }

        private static void TransferToResultsScreen()
        {
            GlobalContext.CurrentGameState = GlobalContext.GameState.Results;
            MusicBox.PlayOnce(AssetManager.MusicTracks.Find(song => song.Name.EndsWith("VictoryJingle")));
        }
    }
}
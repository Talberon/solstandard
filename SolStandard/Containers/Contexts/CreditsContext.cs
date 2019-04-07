using System.Diagnostics;
using SolStandard.Containers.View;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts
{
    public class CreditsContext
    {
        public readonly CreditsView CreditsView;
        private GameContext.GameState previousGameState;
        public const string CreditsPath = "/credits";

        public CreditsContext(CreditsView creditsView)
        {
            CreditsView = creditsView;
        }

        public void OpenView()
        {
            if (GameContext.CurrentGameState == GameContext.GameState.Credits) return;

            Process.Start(GameDriver.SolStandardUrl + CreditsPath);
            previousGameState = GameContext.CurrentGameState;
            GameContext.CurrentGameState = GameContext.GameState.Credits;
        }

        public void ExitView()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GameContext.CurrentGameState = previousGameState;
        }
    }
}
using SolStandard.Containers.View;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts
{
    public class HowToPlayContext
    {
        public ScrollingTextPaneView HowToPlayView { get; }
        private GameContext.GameState previousGameState;
        private const string HowToPlayPath = "/how-to-play";

        public HowToPlayContext()
        {
            HowToPlayView = new HowToPlayView();
        }

        public void OpenView()
        {
            if (GameContext.CurrentGameState == GameContext.GameState.HowToPlay) return;

            previousGameState = GameContext.CurrentGameState;
            GameContext.CurrentGameState = GameContext.GameState.HowToPlay;
        }

        public void ExitView()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GameContext.CurrentGameState = previousGameState;
        }

        public void ScrollWindow(Direction direction)
        {
            HowToPlayView.ScrollContents(direction);
        }

        public void OpenBrowser()
        {
            AssetManager.MenuConfirmSFX.Play();
            CreditsContext.OpenBrowser(GameDriver.SolStandardUrl + HowToPlayPath);
        }
    }
}
using SolStandard.Containers.Components.Credits;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Components.HowToPlay
{
    public class HowToPlayContext
    {
        public ScrollingTextPaneView HowToPlayView { get; }
        private GlobalContext.GameState previousGameState;
        private const string HowToPlayPath = "/how-to-play";

        public HowToPlayContext()
        {
            HowToPlayView = new HowToPlayView();
        }

        public void OpenView()
        {
            if (GlobalContext.CurrentGameState == GlobalContext.GameState.HowToPlay) return;

            previousGameState = GlobalContext.CurrentGameState;
            GlobalContext.CurrentGameState = GlobalContext.GameState.HowToPlay;
        }

        public void ExitView()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GlobalContext.CurrentGameState = previousGameState;
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
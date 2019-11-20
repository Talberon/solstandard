using SolStandard.Containers.View;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts
{
    public class EULAContext
    {
        private const string EULAFileName = "EULA_Confirmed";
        public ScrollingTextPaneView EULAView { get; }

        public bool EULAConfirmed { get; private set; }

        public EULAContext()
        {
            bool? savedEula = GameDriver.SystemFileIO.Load<bool?>(EULAFileName);
            EULAConfirmed = savedEula != null && savedEula == true;
            EULAView = new EULAView();
        }

        public void ConfirmEULAPrompt()
        {
            AssetManager.MenuConfirmSFX.Play();
            GameDriver.SystemFileIO.Save(EULAFileName, true);
            EULAConfirmed = true;
            GameContext.CurrentGameState = GameContext.GameState.MainMenu;
        }

        public void ScrollWindow(Direction direction)
        {
            EULAView.ScrollContents(direction);
        }
    }
}
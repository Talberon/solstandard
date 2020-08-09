using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Components.EULA
{
    public class EULAContext
    {
        private const string EULAFileName = "EULA_Confirmed";
        public ScrollingTextPaneView EULAView { get; }

        public bool EULAConfirmed { get; private set; }

        public EULAContext()
        {
            var savedEula = GameDriver.FileIO.Load<bool?>(EULAFileName);
            EULAConfirmed = savedEula != null && savedEula == true;
            EULAView = new EULAView();
        }

        public void ConfirmEULAPrompt()
        {
            AssetManager.MenuConfirmSFX.Play();
            GameDriver.FileIO.Save(EULAFileName, true);
            EULAConfirmed = true;
            GlobalContext.CurrentGameState = GlobalContext.GameState.MainMenu;
        }

        public void ScrollWindow(Direction direction)
        {
            EULAView.ScrollContents(direction);
        }
    }
}
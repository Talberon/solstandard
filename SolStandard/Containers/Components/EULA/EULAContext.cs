using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;
using SolStandard.NeoGFX.GUI;
using SolStandard.NeoGFX.GUI.Menus;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Components.EULA
{
    public class EULAContext : IGameContext
    {
        private const string EULAFileName = "EULA_Confirmed";
        public ScrollingTextPaneHUD EULAHUD { get; }

        public bool EULAConfirmed { get; private set; }

        public EULAContext()
        {
            var savedEula = GameDriver.FileIO.Load<bool?>(EULAFileName);
            EULAConfirmed = savedEula != null && savedEula == true;
            EULAHUD = new EULAHUD();
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
            EULAHUD.ScrollContents(direction);
        }

        public IHUDView View { get; }
        public MenuContainer MenuContainer { get; }
        public void Update(GameTime gameTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
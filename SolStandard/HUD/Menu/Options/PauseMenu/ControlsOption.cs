using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ControlsOption : MenuOption
    {
        private readonly PauseMenuUI pauseMenuUI;

        public ControlsOption(Color color, PauseMenuUI pauseMenuUI) :
            base(new RenderText(AssetManager.MainMenuFont, "View Controls"), color)
        {
            this.pauseMenuUI = pauseMenuUI;
        }

        public override void Execute()
        {
            //Show Control Scheme
            pauseMenuUI.ChangeMenu(PauseMenuUI.PauseMenus.Controller);
        }
    }
}
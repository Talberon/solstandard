using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class ReturnToPauseMenuOption : MenuOption
    {
        private readonly PauseMenuUI pauseMenuUI;

        public ReturnToPauseMenuOption(Color color, PauseMenuUI pauseMenuUI) : base(
            new RenderText(AssetManager.MainMenuFont, "Back"),
            color
        )
        {
            this.pauseMenuUI = pauseMenuUI;
        }

        public override void Execute()
        {
            pauseMenuUI.ChangeMenu(PauseMenuUI.PauseMenus.Primary);
        }
    }
}
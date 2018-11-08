using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class ReturnToPauseMenuOption : MenuOption
    {
        private readonly PauseScreenView pauseScreenView;

        public ReturnToPauseMenuOption(Color color, PauseScreenView pauseScreenView) : base(
            new RenderText(AssetManager.MainMenuFont, "Back"),
            color
        )
        {
            this.pauseScreenView = pauseScreenView;
        }

        public override void Execute()
        {
            pauseScreenView.ChangeMenu(PauseScreenView.PauseMenus.Primary);
        }

        public override IRenderable Clone()
        {
            return new ReturnToPauseMenuOption(Color, pauseScreenView);
        }
    }
}
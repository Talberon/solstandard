using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ConfigOption : MenuOption
    {
        private readonly PauseScreenView pauseScreenView;

        public ConfigOption(Color color, PauseScreenView pauseScreenView) :
            base(new RenderText(AssetManager.MainMenuFont, "Config"), color)
        {
            this.pauseScreenView = pauseScreenView;
        }

        public override void Execute()
        {
            //Show Options Menu for Sound + Music
            pauseScreenView.ChangeMenu(PauseScreenView.PauseMenus.Config);
        }

        public override IRenderable Clone()
        {
            return new ConfigOption(Color, pauseScreenView);
        }
    }
}
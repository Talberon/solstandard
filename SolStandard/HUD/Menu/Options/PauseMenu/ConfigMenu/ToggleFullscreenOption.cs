using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class ToggleFullscreenOption : MenuOption
    {
        private bool isFullscreen;
        private readonly GameDriver gameDriver;

        public ToggleFullscreenOption(Color color, GameDriver gameDriver) : base(
            new RenderText(AssetManager.MainMenuFont, "Toggle Fullscreen"), color)
        {
            isFullscreen = false;
            this.gameDriver = gameDriver;
        }

        public override void Execute()
        {
            isFullscreen = !isFullscreen;
            if (isFullscreen)
            {
                gameDriver.UseBorderlessFullscreen();
            }
            else
            {
                gameDriver.UseDefaultResolution();
            }
        }

        public override IRenderable Clone()
        {
            return new ToggleFullscreenOption(DefaultColor, gameDriver);
        }
    }
}
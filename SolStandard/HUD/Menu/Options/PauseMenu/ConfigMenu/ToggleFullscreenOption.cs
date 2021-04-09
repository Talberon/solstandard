using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class ToggleFullscreenOption : MenuOption
    {
        private readonly GameDriver gameDriver;

        public ToggleFullscreenOption(Color color, GameDriver gameDriver) : base(
            new RenderText(AssetManager.WindowFont, "Toggle Fullscreen"), color)
        {
            this.gameDriver = gameDriver;
        }

        public override void Execute()
        {
            gameDriver.ToggleFullscreen();
        }

        public override IRenderable Clone()
        {
            return new ToggleFullscreenOption(DefaultColor, gameDriver);
        }
    }
}
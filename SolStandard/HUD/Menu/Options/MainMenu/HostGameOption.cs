using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class HostGameOption : MenuOption
    {
        private const string HostGameOptionText = "Host Game";

        public HostGameOption(Color windowColor) :
            base(new RenderText(AssetManager.MainMenuFont, HostGameOptionText), windowColor)
        {
        }

        public override void Execute()
        {
            GameDriver.HostGame();
        }

        public override IRenderable Clone()
        {
            return new HostGameOption(DefaultColor);
        }
    }
}
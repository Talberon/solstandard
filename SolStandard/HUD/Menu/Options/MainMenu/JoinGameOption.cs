using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class JoinGameOption : MenuOption
    {
        private const string JoinGameOptionText = "Join Game";

        public JoinGameOption(Color windowColor) :
            base(new RenderText(AssetManager.MainMenuFont, JoinGameOptionText), windowColor)
        {
        }

        public override void Execute()
        {
            GameDriver.JoinGame();
        }

        public override IRenderable Clone()
        {
            return new JoinGameOption(DefaultColor);
        }
    }
}
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
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
            //TODO Update screen to show "Waiting for Connection"
            //TODO Give user a way to set the IP address they will connect to
        }

        public override IRenderable Clone()
        {
            return new JoinGameOption(DefaultColor);
        }
    }
}
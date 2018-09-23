using Microsoft.Xna.Framework;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class QuitGameOption : MenuOption
    {
        private const string NewGameOptionText = "Quit Game ";

        public QuitGameOption(Color windowColor) : base(windowColor, NewGameOptionText, AssetManager.MainMenuFont)
        {
        }

        public override void Execute()
        {
            GameDriver.QuitGame();
        }
    }
}
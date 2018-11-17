using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class QuitGameOption : MenuOption
    {
        private const string QuitGameOptionText = "Quit Game";

        public QuitGameOption(Color windowColor) : base(new RenderText(AssetManager.MainMenuFont, QuitGameOptionText), windowColor)
        {
        }

        public override void Execute()
        {
            GameDriver.QuitGame();
        }

        public override IRenderable Clone()
        {
            return new QuitGameOption(DefaultColor);
        }
    }
}
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class QuitGameOption : IOption
    {
        private const string NewGameOptionText = "Quit Game ";

        public string LabelText { get; private set; }
        public IRenderable OptionWindow { get; private set; }

        public QuitGameOption(ITexture2D windowTexture)
        {
            LabelText = NewGameOptionText;
            OptionWindow = new Window.Window(
                "Option " + LabelText,
                windowTexture,
                new RenderText(GameDriver.MainMenuFont, LabelText),
                Color.White
            );
        }

        public void Execute()
        {
            GameDriver.QuitGame();
        }
    }
}
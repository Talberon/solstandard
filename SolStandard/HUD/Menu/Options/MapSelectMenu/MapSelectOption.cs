using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;

namespace SolStandard.HUD.Menu.Options.MapSelectMenu
{
    public class MapSelectOption : IOption
    {
        private readonly string fileName;
        public string LabelText { get; private set; }
        public IRenderable OptionWindow { get; private set; }

        public MapSelectOption(string mapTitle, string fileName)
        {
            this.fileName = fileName;
            LabelText = mapTitle;
            OptionWindow = new Window.Window(
                "MapSelectOption " + LabelText,
                GameDriver.WindowTexture,
                new RenderText(GameDriver.MainMenuFont, LabelText),
                Color.White
            );
        }

        public void Execute()
        {
            GameDriver.NewGame(fileName);
        }
    }
}
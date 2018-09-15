using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;

namespace SolStandard.HUD.Menu.Options.MapSelectMenu
{
    public class MapSelectOption : IOption
    {
        private readonly MapInfo mapInfo;
        public string LabelText { get; private set; }
        public IRenderable OptionWindow { get; private set; }

        public MapSelectOption(MapInfo mapInfo)
        {
            this.mapInfo = mapInfo;
            LabelText = mapInfo.Title;
            OptionWindow = new Window.Window(
                "MapSelectOption " + LabelText,
                GameDriver.WindowTexture,
                new RenderText(GameDriver.MainMenuFont, LabelText),
                Color.White
            );
        }

        public void Execute()
        {
            GameDriver.NewGame(mapInfo.FileName);
        }
    }
}
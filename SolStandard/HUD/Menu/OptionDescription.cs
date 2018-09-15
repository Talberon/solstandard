using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu
{
    public class OptionDescription
    {
        private readonly Window.Window infoTitleWindow;
        private readonly Window.Window infoDescriptionWindow;
        private readonly ITexture2D windowTexture;

        public OptionDescription(ITexture2D windowTexture, string title, string description)
        {
            this.windowTexture = windowTexture;
            infoTitleWindow =
                new Window.Window("Title", windowTexture, new RenderText(GameDriver.HeaderFont, title), Color.White);
            infoDescriptionWindow =
                new Window.Window("Description", windowTexture, new RenderText(GameDriver.WindowFont, description),
                    Color.White);
        }

        public IRenderable MenuInfoWindow
        {
            get
            {
                WindowContentGrid infoWindowContentGrid = new WindowContentGrid
                (
                    new IRenderable[,]
                    {
                        {infoTitleWindow},
                        {infoDescriptionWindow}
                    },
                    2
                );
                
                return new Window.Window("Info Window", windowTexture, infoWindowContentGrid, Color.White);
            }
        }
    }
}
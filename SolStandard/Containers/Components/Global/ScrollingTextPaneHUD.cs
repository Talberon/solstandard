using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.MainMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Components.Global
{
    public class ScrollingTextPaneHUD : IUserInterface
    {
        private const int WindowSpacing = 10;
        private readonly ScrollableWindow textWindow;
        private readonly Window controlWindow;

        protected ScrollingTextPaneHUD(ISpriteFont windowFont, string bigTextContent, IRenderable controlInfo = null)
        {
            controlWindow = new Window(controlInfo ?? new WindowContentGrid(new IRenderable[,]
            {
                {
                    new WindowContentGrid(new[,]
                    {
                        {
                            new RenderText(windowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.CursorUp, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorLeft, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorDown, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorRight, GameDriver.CellSize),
                            new RenderText(windowFont, " to scroll.")
                        }
                    })
                }
            }), MainMenuHUD.MenuColor);

            textWindow = new ScrollableWindow(
                new RenderText(AssetManager.WindowFont, bigTextContent),
                GameDriver.ScreenSize / 1.5f,
                MainMenuHUD.MenuColor
            );
        }

        public void ScrollContents(Direction direction)
        {
            const int scrollSpeed = 15;
            textWindow.ScrollWindowContents(direction, scrollSpeed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 licensePosition =
                (GameDriver.ScreenSize / 2) - (new Vector2(textWindow.Width, textWindow.Height) / 2);

            controlWindow.Draw(spriteBatch, licensePosition - new Vector2(0, controlWindow.Height + WindowSpacing));
            textWindow.Draw(spriteBatch, licensePosition);
        }
    }
}
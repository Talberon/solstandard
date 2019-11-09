using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.View
{
    public class EULAView : IUserInterface
    {
        private const int WindowSpacing = 10;
        private readonly ScrollableWindow eulaWindow;
        private readonly Window controlWindow;

        public EULAView()
        {
            ISpriteFont windowFont = AssetManager.WindowFont;
            controlWindow = new Window(new WindowContentGrid(new IRenderable[,]
            {
                {
                    new WindowContentGrid(new[,]
                    {
                        {
                            new RenderText(windowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.Confirm, GameDriver.CellSize),
                            new RenderText(windowFont, " to accept the agreement."),
                        }
                    })
                },
                {
                    new WindowContentGrid(new[,]
                    {
                        {
                            new RenderText(windowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.CursorUp, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorLeft, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorDown, GameDriver.CellSize),
                            InputIconProvider.GetInputIcon(Input.CursorRight, GameDriver.CellSize),
                            new RenderText(windowFont, " to scroll."),
                        }
                    })
                }
            }), MainMenuView.MenuColor);

            eulaWindow = new ScrollableWindow(
                new RenderText(AssetManager.WindowFont, AssetManager.EULAText),
                GameDriver.ScreenSize / 1.5f,
                MainMenuView.MenuColor
            );
        }

        public void ScrollContents(Direction direction)
        {
            const int scrollSpeed = 15;
            eulaWindow.ScrollWindowContents(direction, scrollSpeed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 licensePosition =
                (GameDriver.ScreenSize / 2) - (new Vector2(eulaWindow.Width, eulaWindow.Height) / 2);

            controlWindow.Draw(spriteBatch, licensePosition - new Vector2(0, controlWindow.Height + WindowSpacing));
            eulaWindow.Draw(spriteBatch, licensePosition);
        }
    }
}
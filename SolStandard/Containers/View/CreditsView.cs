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
    public class CreditsView : IUserInterface
    {
        private const int WindowSpacing = 10;
        private readonly ScrollableWindow creditsWindow;
        private readonly Window controlWindow;

        public CreditsView()
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
                            new RenderText(windowFont, " to view credits in browser."),
                        },
                        {
                            new RenderText(windowFont, "Press"),
                            InputIconProvider.GetInputIcon(Input.Cancel, GameDriver.CellSize),
                            new RenderText(windowFont, " to return to menu."),
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

            creditsWindow = new ScrollableWindow(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {new RenderText(AssetManager.WindowFont, AssetManager.CreditsText)}
                    },
                    1,
                    HorizontalAlignment.Centered
                ),
                GameDriver.ScreenSize / 1.5f,
                MainMenuView.MenuColor
            );
        }

        public void ScrollContents(Direction direction)
        {
            const int scrollSpeed = 15;
            creditsWindow.ScrollWindowContents(direction, scrollSpeed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            controlWindow.Draw(spriteBatch, CreditsCenter() - new Vector2(0, controlWindow.Height + WindowSpacing));
            creditsWindow.Draw(spriteBatch, CreditsCenter());
        }

        private Vector2 CreditsCenter()
        {
            Vector2 screenCenter = GameDriver.ScreenSize / 2;
            Vector2 halfWindowSize = new Vector2(creditsWindow.Width, creditsWindow.Height) / 2;
            return screenCenter - halfWindowSize;
        }
    }
}
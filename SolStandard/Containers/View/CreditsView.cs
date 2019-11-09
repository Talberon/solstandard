using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Monogame;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;

namespace SolStandard.Containers.View
{
    public class CreditsView : IUserInterface
    {
        private readonly ScrollableWindow creditsWindow;
        private readonly SpriteAtlas background;

        public CreditsView()
        {
            ISpriteFont windowFont = AssetManager.WindowFont;

            creditsWindow = new ScrollableWindow(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {new RenderText(windowFont, "Full credits are available at")},
                        {new RenderText(windowFont, GameDriver.SolStandardUrl + CreditsContext.CreditsPath)},
                        {
                            new WindowContentGrid(new[,]
                            {
                                {
                                    new RenderText(windowFont, "Press"),
                                    InputIconProvider.GetInputIcon(Input.Confirm, GameDriver.CellSize),
                                    new RenderText(windowFont, " to continue in browser, or"),
                                    InputIconProvider.GetInputIcon(Input.Cancel, GameDriver.CellSize),
                                    new RenderText(windowFont, " to return.")
                                }
                            })
                        },
                        {new RenderText(AssetManager.WindowFont, AssetManager.CreditsText)}
                    },
                    1,
                    HorizontalAlignment.Centered
                ),
                GameDriver.ScreenSize / 1.5f,
                MainMenuView.MenuColor
            );
            background = new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                new Vector2(GameDriver.ScreenSize.X, GameDriver.ScreenSize.Y));
        }


        public void ScrollContents(Direction direction)
        {
            const int scrollSpeed = 15;
            creditsWindow.ScrollWindowContents(direction, scrollSpeed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawBackground(spriteBatch);
            creditsWindow.Draw(spriteBatch, CreditsCenter());
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            Vector2 centerScreen = GameDriver.ScreenSize / 2;
            Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
            background.Draw(spriteBatch, centerScreen - backgroundCenter);
        }

        private Vector2 CreditsCenter()
        {
            Vector2 screenCenter = GameDriver.ScreenSize / 2;
            Vector2 halfWindowSize = new Vector2(creditsWindow.Width, creditsWindow.Height) / 2;
            return screenCenter - halfWindowSize;
        }
    }
}
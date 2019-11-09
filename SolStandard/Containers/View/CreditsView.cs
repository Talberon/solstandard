using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;

namespace SolStandard.Containers.View
{
    public class CreditsView : IUserInterface
    {
        private readonly ScrollableWindow creditsWindow;
        private readonly SpriteAtlas background;

        public CreditsView()
        {
            creditsWindow = new ScrollableWindow(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {new RenderText(AssetManager.MainMenuFont, "1Full credits are available at")},
                        {new RenderText(AssetManager.MainMenuFont,GameDriver.SolStandardUrl + CreditsContext.CreditsPath)},
                        {new RenderText(AssetManager.MainMenuFont, "F2ull credits are available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Fu3ll credits are available at")},
                        {new RenderText(AssetManager.MainMenuFont,GameDriver.SolStandardUrl + CreditsContext.CreditsPath)},
                        {new RenderText(AssetManager.MainMenuFont, "Ful4l credits are available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full5 credits are available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full 6credits are available at")},
                        {new RenderText(AssetManager.MainMenuFont,GameDriver.SolStandardUrl + CreditsContext.CreditsPath)},
                        {new RenderText(AssetManager.MainMenuFont, "Full c7redits are available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full cr8edits are available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full cre9dits are available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full cred10its are available at")},
                        {new RenderText(AssetManager.MainMenuFont,GameDriver.SolStandardUrl + CreditsContext.CreditsPath)},
                        {new RenderText(AssetManager.MainMenuFont, "Full credit11s are available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits 12are available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits ar13e available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits are 14available at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits are av15ailable at")},
                        {new RenderText(AssetManager.MainMenuFont,GameDriver.SolStandardUrl + CreditsContext.CreditsPath)},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits are avai16lable at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits are availa17ble at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits are availabl18e at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits are available 19at")},
                        {new RenderText(AssetManager.MainMenuFont, "Full credits are available at20")},
                        {new RenderText(AssetManager.MainMenuFont,GameDriver.SolStandardUrl + CreditsContext.CreditsPath)},
                        {new RenderText(AssetManager.MainMenuFont,"Press confirm to continue in browser, or cancel to return.")}
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
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;

namespace SolStandard.Containers.View
{
    public class CreditsView : IUserInterface
    {
        private bool visible;
        private readonly Window creditsWindow;
        private readonly SpriteAtlas background;

        public CreditsView()
        {
            creditsWindow = new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(AssetManager.MainMenuFont, "Full credits are available at")
                        },
                        {
                            new RenderText(AssetManager.MainMenuFont,
                                GameDriver.SolStandardUrl + CreditsContext.CreditsPath
                            )
                        },
                        {
                            new RenderText(AssetManager.MainMenuFont,
                                "Press confirm to continue in browser, or cancel to return."
                            )
                        }
                    },
                    1,
                    HorizontalAlignment.Centered
                ),
                MainMenuView.MenuColor,
                HorizontalAlignment.Centered
            );
            background = new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                new Vector2(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));

            visible = true;
        }

        public void ToggleVisible()
        {
            visible = !visible;
        }

        private Vector2 CreditsCenter()
        {
            Vector2 screenCenter = GameDriver.ScreenSize / 2;
            Vector2 halfWindowSize = new Vector2(creditsWindow.Width, creditsWindow.Height) / 2;
            return screenCenter - halfWindowSize;
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            Vector2 centerScreen = GameDriver.ScreenSize / 2;
            Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
            background.Draw(spriteBatch, centerScreen - backgroundCenter);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawBackground(spriteBatch);
            creditsWindow.Draw(spriteBatch, CreditsCenter());
        }
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class CreditsView : IUserInterface
    {
        private bool visible;
        private readonly Window creditsWindow;

        public CreditsView()
        {
            creditsWindow = new Window(
                new RenderText(AssetManager.MainMenuFont,
                    "See credits @ " + GameDriver.SolStandardUrl + CreditsContext.CreditsPath + Environment.NewLine +
                    "Press confirm or cancel to return."
                ),
                MainMenuView.MenuColor,
                HorizontalAlignment.Centered
            );

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

        public void Draw(SpriteBatch spriteBatch)
        {
            creditsWindow.Draw(spriteBatch, CreditsCenter());
        }
    }
}
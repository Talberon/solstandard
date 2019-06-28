using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class BackgroundView : IUserInterface
    {
        private readonly SpriteAtlas background;
        private bool IsVisible { get; set; }

        public BackgroundView()
        {
            IsVisible = true;
            background = new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                GameDriver.ScreenSize);
        }

        public void ToggleVisible()
        {
            IsVisible = !IsVisible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
            Vector2 centerScreen = GameDriver.ScreenSize / 2;
            Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
            background.Draw(spriteBatch, centerScreen - backgroundCenter);
        }
    }
}
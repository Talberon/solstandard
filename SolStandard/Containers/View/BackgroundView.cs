using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class BackgroundView : IUserInterface
    {
        private static SpriteAtlas Background =>
            new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                GameDriver.ScreenSize);

        private bool IsVisible { get; set; }

        public BackgroundView()
        {
            IsVisible = true;
        }

        public void ToggleVisible()
        {
            IsVisible = !IsVisible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
            Vector2 centerScreen = GameDriver.ScreenSize / 2;
            Vector2 backgroundCenter = new Vector2(Background.Width, Background.Height) / 2;
            Background.Draw(spriteBatch, centerScreen - backgroundCenter);
        }
    }
}
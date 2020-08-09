using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Components.Global
{
    public class StaticBackgroundView : IUserInterface
    {
        private static readonly Color RenderColor = new Color(100, 100, 100);

        private static SpriteAtlas Background =>
            new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                ScaleHeightToWidth(
                    new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                    GameDriver.ScreenSize.X));

        private static Vector2 ScaleHeightToWidth(Vector2 sourceProportions, float width)
        {
            var scaledProportions = new Vector2 {X = width, Y = sourceProportions.Y * width / sourceProportions.X};
            return scaledProportions;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 centerScreen = GameDriver.ScreenSize / 2;
            Vector2 backgroundCenter = new Vector2(Background.Width, Background.Height) / 2;
            Background.Draw(spriteBatch, centerScreen - backgroundCenter, RenderColor);
        }
    }
}
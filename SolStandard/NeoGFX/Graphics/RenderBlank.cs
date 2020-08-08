using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.NeoGFX.GUI;

namespace SolStandard.NeoGFX.Graphics
{
    public class RenderBlank : IPositionedRenderable, IWindowContent
    {
        public static RenderBlank Blank => new RenderBlank(0, 0);

        public static RenderBlank WithDimensions(Vector2 dimensions)
        {
            (float width, float height) = dimensions;
            return new RenderBlank(width, height);
        }

        private RenderBlank(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width { get; }
        public float Height { get; }

        public void Update(GameTime gameTime)
        {
            //Do nothing
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Do nothing
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            //Do nothing
        }

        public Vector2 TopLeftPoint { get; set; }
        public Vector2 ScreenCoordinates { get; set; }
    }
}
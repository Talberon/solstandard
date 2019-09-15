using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content
{
    public class RenderBlank : IRenderable
    {
        public static IRenderable Blank { get; } = new RenderBlank();
        
        public int Height { get; }
        public int Width { get; }
        public Color DefaultColor { get; set; }

        private RenderBlank()
        {
            Height = 0;
            Width = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            //Do nothing
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            //Do nothing
        }

        public IRenderable Clone()
        {
            return Blank;
        }
    }
}
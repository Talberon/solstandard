using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content
{
    public class RenderBlank : IRenderable
    {
        public int Height { get; private set; }
        public int Width { get; private set; }

        public RenderBlank()
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
            return new RenderBlank();
        }
    }
}
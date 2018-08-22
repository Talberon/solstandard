using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content
{
    public class WindowContent : IRenderable
    {
        //TODO extend this for healthbar, images or text (with font)

        private readonly IRenderable content;

        public WindowContent(IRenderable content)
        {
            this.content = content;
        }

        public int Height
        {
            get { return content.Height; }
        }

        public int Width
        {
            get { return content.Width; }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            content.Draw(spriteBatch, position);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            content.Draw(spriteBatch, position, color);
        }
    }
}
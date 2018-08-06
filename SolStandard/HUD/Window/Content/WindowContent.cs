using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content
{
    public abstract class WindowContent : IRenderable
    {
        //TODO extend this for healthbar, images or text (with font)

        private readonly IRenderable content;

        protected WindowContent(IRenderable content)
        {
            this.content = content;
        }

        public int GetHeight()
        {
            return content.GetHeight();
        }

        public int GetWidth()
        {
            return content.GetWidth();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            content.Draw(spriteBatch, position);
        }
    }
}
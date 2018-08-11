using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content
{
    public class WindowContent : IRenderable
    {
        //TODO extend this for healthbar, images or text (with font)

        private readonly IRenderable image;

        public WindowContent(IRenderable image)
        {
            this.image = image;
        }

        public int GetHeight()
        {
            return image.GetHeight();
        }

        public int GetWidth()
        {
            return image.GetWidth();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            image.Draw(spriteBatch, position);
        }
    }
}
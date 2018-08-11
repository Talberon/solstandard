using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window.Content
{
    public class RenderText : IRenderable
    {
        private readonly ISpriteFont font;
        private readonly string message;

        public RenderText(ISpriteFont font, string message)
        {
            this.font = font;
            this.message = message;
        }

        public int GetHeight()
        {
            return (int) font.MeasureString(message).Y;
        }

        public int GetWidth()
        {
            return (int) font.MeasureString(message).X;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.DrawString(font.GetSpriteFont(), message, position, Color.White);
        }
    }
}
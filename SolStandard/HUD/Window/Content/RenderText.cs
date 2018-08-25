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

        public int Height
        {
            get { return (int) font.MeasureString(message).Y; }
        }

        public int Width
        {
            get { return (int) font.MeasureString(message).X; }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.DrawString(font.MonoGameSpriteFont, message, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font.MonoGameSpriteFont, message, position, color);
        }
    }
}
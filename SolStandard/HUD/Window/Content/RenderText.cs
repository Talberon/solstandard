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
        private const string Space = " ";
        public Color DefaultColor { get; set; }

        public RenderText(ISpriteFont font, string message, Color color)
        {
            this.font = font;
            this.message = message;
            DefaultColor = color;
        }

        public RenderText(ISpriteFont font, string message) : this(font, message, Color.White)
        {
            //Intentionally left blank
        }

        public int Height => (int) font.MeasureString(message).Y;

        public int Width => (int) font.MeasureString(message + Space).X;

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            spriteBatch.DrawString(font.MonoGameSpriteFont, message, position, colorOverride);
        }

        public IRenderable Clone()
        {
            return new RenderText(font, message, DefaultColor);
        }
    }
}
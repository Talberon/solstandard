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
        private readonly Color defaultColor;

        public RenderText(ISpriteFont font, string message, Color defaultColor)
        {
            this.font = font;
            this.message = message;
            this.defaultColor = defaultColor;
        }

        public RenderText(ISpriteFont font, string message) : this(font, message, Color.White)
        {
            //Intentionally left blank
        }

        public int Height
        {
            get { return (int) font.MeasureString(message).Y; }
        }

        public int Width
        {
            get { return (int) font.MeasureString(message + Space).X; }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, defaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            spriteBatch.DrawString(font.MonoGameSpriteFont, message, position, colorOverride);
        }
    }
}
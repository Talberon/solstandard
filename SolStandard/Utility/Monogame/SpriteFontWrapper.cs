using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    public class SpriteFontWrapper : ISpriteFont
    {
        private readonly SpriteFont font;

        public SpriteFontWrapper(SpriteFont font)
        {
            this.font = font;
        }

        public Vector2 MeasureString(string text)
        {
            return font.MeasureString(text);
        }

        public SpriteFont GetSpriteFont()
        {
            return font;
        }
    }
}
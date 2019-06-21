using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    public class SpriteFontWrapper : ISpriteFont
    {
        public SpriteFontWrapper(SpriteFont font)
        {
            MonoGameSpriteFont = font;
        }

        public Vector2 MeasureString(string text)
        {
            return MonoGameSpriteFont.MeasureString(text);
        }

        public SpriteFont MonoGameSpriteFont { get; }
    }
}
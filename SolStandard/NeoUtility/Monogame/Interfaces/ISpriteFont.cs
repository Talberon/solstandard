using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Steelbreakers.Utility.Monogame.Interfaces
{
    public interface ISpriteFont
    {
        Vector2 MeasureString(string text);
        SpriteFont MonoGameSpriteFont { get; }
    }
}
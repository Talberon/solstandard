using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.NeoUtility.Monogame.Interfaces
{
    public interface ISpriteFont
    {
        Vector2 MeasureString(string text);
        SpriteFont MonoGameSpriteFont { get; }
    }
}
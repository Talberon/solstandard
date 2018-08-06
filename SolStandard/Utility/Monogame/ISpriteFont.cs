using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility.Monogame
{
    public interface ISpriteFont
    {
        Vector2 MeasureString(string text);
        SpriteFont GetSpriteFont();
    }
}
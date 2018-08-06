using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility
{
    public interface IRenderable
    {
        int GetHeight();
        int GetWidth();
        void Draw(SpriteBatch spriteBatch, Vector2 position);
    }
}

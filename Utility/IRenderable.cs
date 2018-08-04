using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility
{
    public interface IRenderable
    {
        void Draw(SpriteBatch spriteBatch, Vector2 position);
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility
{
    interface IRenderable
    {
        void Draw(SpriteBatch spriteBatch, Vector2 position);
    }
}

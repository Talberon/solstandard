using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Steelbreakers.Utility.Graphics
{
    public interface IRenderable
    {
        float Width { get; }
        float Height { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.NeoGFX.Graphics
{
    public interface INeoRenderable
    {
        float Width { get; }
        float Height { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
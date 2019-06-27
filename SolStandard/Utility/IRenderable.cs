using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility
{
    public interface IRenderable
    {
        int Height { get; }
        int Width { get; }
        void Draw(SpriteBatch spriteBatch, Vector2 position);
        void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride);
        Color DefaultColor { get; set; }
        IRenderable Clone();
    }
}

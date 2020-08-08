using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.NeoGFX.Graphics;

namespace SolStandard.NeoGFX.GUI
{
    public interface IWindowContent : IRenderable
    {
        void Draw(SpriteBatch spriteBatch, Vector2 coordinates);
    }
}
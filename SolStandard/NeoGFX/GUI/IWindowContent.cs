using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.NeoGFX.Graphics;

namespace SolStandard.NeoGFX.GUI
{
    public interface IWindowContent : INeoRenderable
    {
        void Draw(SpriteBatch spriteBatch, Vector2 coordinates);
    }
}
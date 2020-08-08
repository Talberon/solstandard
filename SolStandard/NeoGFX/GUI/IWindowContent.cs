using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steelbreakers.Utility.Graphics;

namespace Steelbreakers.Utility.GUI.HUD
{
    public interface IWindowContent : IRenderable
    {
        void Draw(SpriteBatch spriteBatch, Vector2 coordinates);
    }
}
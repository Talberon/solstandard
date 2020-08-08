using Microsoft.Xna.Framework;
using Steelbreakers.Utility.Graphics;

namespace Steelbreakers.Utility.GUI.HUD
{
    public interface IWindow : IRenderable
    {
        Vector2 CurrentPosition { get; set; }
    }
}
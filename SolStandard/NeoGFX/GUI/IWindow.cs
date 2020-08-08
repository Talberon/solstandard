using Microsoft.Xna.Framework;
using SolStandard.NeoGFX.Graphics;

namespace SolStandard.NeoGFX.GUI
{
    public interface IWindow : IRenderable
    {
        Vector2 CurrentPosition { get; set; }
    }
}
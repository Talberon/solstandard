using Microsoft.Xna.Framework;
using SolStandard.NeoGFX.Graphics;

namespace SolStandard.NeoGFX.GUI
{
    public interface IWindow : INeoRenderable
    {
        Vector2 CurrentPosition { get; set; }
    }
}
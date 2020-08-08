using System.Collections.Generic;
using SolStandard.NeoGFX.Graphics;

namespace SolStandard.NeoGFX.GUI
{
    public interface IHUDView : IRenderable
    {
        List<IWindow> Windows { get; }
    }
}
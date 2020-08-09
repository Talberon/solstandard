using System.Collections.Generic;
using SolStandard.NeoGFX.Graphics;

namespace SolStandard.NeoGFX.GUI
{
    public interface IHUDView : INeoRenderable
    {
        List<IWindow> Windows { get; }
    }
}
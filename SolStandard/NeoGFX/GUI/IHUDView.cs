using System.Collections.Generic;
using Steelbreakers.Utility.Graphics;
using Steelbreakers.Utility.GUI.HUD;

namespace Steelbreakers.Contexts.Components.Views
{
    public interface IHUDView : IRenderable
    {
        List<IWindow> Windows { get; }
    }
}
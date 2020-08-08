using Microsoft.Xna.Framework;
using Steelbreakers.Utility.GUI.HUD;

namespace Steelbreakers.Utility.Graphics
{
    public static class WindowContentExtensions
    {
        public static Window.Builder ToWindowBuilder(this IWindowContent me)
        {
            return new Window.Builder().Content(me);
        }

        public static Window ToWindow(this IWindowContent me, Color? windowColor = null,
            WindowBorder borderStyle = WindowBorder.Rounded, int? paddingPx = null)
        {
            Window.Builder builder = new Window.Builder().Content(me);

            if (windowColor is object) builder = builder.WindowColor(windowColor.Value);
            if (paddingPx is object) builder.InsidePadding(paddingPx.Value);

            builder = builder.BorderStyle(borderStyle);

            return builder.Build();
        }
    }
}
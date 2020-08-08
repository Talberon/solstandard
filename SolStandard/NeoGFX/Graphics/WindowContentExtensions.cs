using Microsoft.Xna.Framework;
using SolStandard.NeoGFX.GUI;

namespace SolStandard.NeoGFX.Graphics
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
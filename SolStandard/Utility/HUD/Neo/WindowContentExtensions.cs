using Microsoft.Xna.Framework;

namespace SolStandard.Utility.HUD.Neo
{
    public static class WindowContentExtensions
    {
        public static NeoWindow.Builder ToWindowBuilder(this IRenderable me)
        {
            return new NeoWindow.Builder().Content(me);
        }

        public static NeoWindow ToWindow(this IRenderable me, Color? windowColor = null,
            WindowBorder borderStyle = WindowBorder.Pixel, int? paddingPx = null)
        {
            NeoWindow.Builder builder = new NeoWindow.Builder().Content(me);

            if (windowColor is object) builder = builder.WindowColor(windowColor.Value);
            if (paddingPx is object) builder.InsidePadding(paddingPx.Value);

            builder = builder.BorderStyle(borderStyle);

            return builder.Build();
        }
    }
}
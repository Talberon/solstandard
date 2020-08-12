using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SolStandard.HUD.Window;

namespace SolStandard.Utility.HUD.Neo
{
    public static class HUDUtils
    {
        public static Vector2 TopLeftOf(this IWindow _, RectangleF region, float padding = 0f)
        {
            return new Vector2(region.Left + padding, region.Y + padding);
        }

        public static Vector2 TopRightOf(this IWindow me, RectangleF region, float padding = 0f)
        {
            float horizontalPosition = region.Right - me.Width;
            return new Vector2(horizontalPosition - padding, region.Y + padding);
        }

        public static Vector2 BottomLeftOf(this IWindow me, RectangleF region, float padding = 0f)
        {
            float verticalPosition = region.Bottom - me.Height;
            return new Vector2(region.Left + padding, verticalPosition - padding);
        }

        public static Vector2 BottomRightOf(this IWindow me, RectangleF region, float padding = 0f)
        {
            float horizontalPosition = region.Right - me.Width;
            float verticalPosition = region.Bottom - me.Height;
            return new Vector2(horizontalPosition - padding, verticalPosition - padding);
        }

        public static Vector2 CenterOf(this IWindow me, RectangleF region)
        {
            float horizontalPosition = region.Center.X - me.HalfSize().X;
            float verticalPosition = region.Center.Y - me.HalfSize().Y;
            return new Vector2(horizontalPosition, verticalPosition);
        }

        public static Vector2 TopCenterOf(this IWindow me, RectangleF region, float padding = 0f)
        {
            return new Vector2(me.CenterOf(region).X, region.Top + padding);
        }

        public static Vector2 BottomCenterOf(this IWindow me, RectangleF region, float padding = 0f)
        {
            float verticalPosition = region.Bottom - me.Height;
            return new Vector2(me.CenterOf(region).X, verticalPosition - padding);
        }

        public static Vector2 RightCenterOf(this IWindow me, RectangleF region, float padding = 0f)
        {
            float horizontalPosition = region.Right - me.Width;
            return new Vector2(horizontalPosition - padding, me.CenterOf(region).Y);
        }

        public static Vector2 LeftCenterOf(this IWindow me, RectangleF region, float padding = 0f)
        {
            return new Vector2(region.Left + padding, me.CenterOf(region).Y);
        }

        public static Vector2 Size(this IWindow me)
        {
            return new Vector2(me.Width, me.Height);
        }

        public static Vector2 HalfSize(this IWindow me)
        {
            return Size(me) / 2;
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;
using SolStandard.Utility;

namespace SolStandard.Containers.Components.Global
{
    public static class GlobalHUDUtils
    {
        private const int WindowPadding = 10;
        private const int NotificationDurationInFrames = 180;

        private static readonly List<HudNotification> Notifications = new List<HudNotification>();
        private static int _notificationTimer;

        public static void AddNotification(string message)
        {
            _notificationTimer = 0;
            Notifications.Add(new HudNotification(message));
        }

        private static Vector2 BottomRightOfScreen(IRenderable contentToPosition, float verticalOffset)
        {
            var extraOffset = new Vector2(0, verticalOffset);
            var screenEdgePadding = new Vector2(10, 20);
            
            return GameDriver.ScreenSize
                   - new Vector2(contentToPosition.Width, contentToPosition.Height)
                   - extraOffset
                   - screenEdgePadding;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            int verticalOffset = 0;
            foreach (HudNotification currentNotification in Notifications)
            {
                currentNotification.Draw(spriteBatch, BottomRightOfScreen(currentNotification, verticalOffset));
                verticalOffset += currentNotification.Height + WindowPadding;
            }

            _notificationTimer++;
            if (_notificationTimer % NotificationDurationInFrames != 0) return;
            if (Notifications.Count > 0) Notifications.RemoveAt(0);
            _notificationTimer = 0;
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;

namespace SolStandard.Containers
{
    public class WindowLayer : IGameLayer
    {
        private readonly Vector2 screenSize;
        private const int WindowEdgeBuffer = 20;

        public Window DebugWindow { get; set; }

        public Window LeftUnitPortraitWindow { get; set; }
        public Window LeftUnitDetailWindow { get; set; }

        public Window RightUnitPortraitWindow { get; set; }
        public Window RightUnitDetailWindow { get; set; }

        public List<Window> ExtraWindows { get; set; }

        public WindowLayer(Vector2 screenSize)
        {
            this.screenSize = screenSize;
            ExtraWindows = new List<Window>();
        }

        private Vector2 LeftUnitPortraitWindowPosition(int portraitWindowHeight)
        {
            return new Vector2(WindowEdgeBuffer, screenSize.Y - portraitWindowHeight);
        }

        private Vector2 LeftUnitDetailWindowPosition(int detailWindowHeight, int leftPortraitWindowWidth)
        {
            return new Vector2(WindowEdgeBuffer + leftPortraitWindowWidth, screenSize.Y - detailWindowHeight);
        }

        private Vector2 RightUnitPortraitWindowPosition(int detailWindowHeight, int portraitWindowWidth)
        {
            return new Vector2(screenSize.X - portraitWindowWidth - WindowEdgeBuffer,
                screenSize.Y - detailWindowHeight);
        }

        private Vector2 RightUnitDetailWindowPosition(int detailWindowHeight, int detailWindowWidth,
            int rightPortraitWidth)
        {
            return new Vector2(screenSize.X - detailWindowWidth - rightPortraitWidth - WindowEdgeBuffer,
                screenSize.Y - detailWindowHeight);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Window window in ExtraWindows)
            {
                //TODO Figure out where to draw these
                window.Draw(spriteBatch, new Vector2(0));
            }

            //TODO Turn this off eventually or add a debug mode flag
            DebugWindow.Draw(spriteBatch, new Vector2(0));

            if (LeftUnitPortraitWindow != null)
            {
                LeftUnitPortraitWindow.Draw(spriteBatch,
                    LeftUnitPortraitWindowPosition(LeftUnitPortraitWindow.GetHeight()));

                if (LeftUnitDetailWindow != null)
                {
                    LeftUnitDetailWindow.Draw(spriteBatch,
                        LeftUnitDetailWindowPosition(LeftUnitDetailWindow.GetHeight(),
                            LeftUnitPortraitWindow.GetWidth()));
                }
            }

            if (RightUnitPortraitWindow != null)
            {
                RightUnitPortraitWindow.Draw(spriteBatch, RightUnitPortraitWindowPosition(RightUnitPortraitWindow.GetHeight(),
                    RightUnitPortraitWindow.GetWidth()));

                if (RightUnitDetailWindow != null)
                {
                    RightUnitDetailWindow.Draw(spriteBatch,
                        RightUnitDetailWindowPosition(RightUnitDetailWindow.GetHeight(),
                            RightUnitDetailWindow.GetWidth(), RightUnitPortraitWindow.GetWidth()));
                }
            }
        }
    }
}
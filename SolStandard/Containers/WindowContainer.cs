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

        public Window LeftUnitSelectionWindow { get; set; }
        public Window RightUnitSelectionWindow { get; set; }
        public List<Window> ExtraWindows { get; set; }

        public WindowLayer(Vector2 screenSize)
        {
            this.screenSize = screenSize;
            ExtraWindows = new List<Window>();
        }

        private Vector2 LeftUnitSelectionWindowPosition(int windowHeight)
        {
            return new Vector2(WindowEdgeBuffer, screenSize.Y - windowHeight);
        }

        private Vector2 RightUnitSelectionWindowPosition(int windowHeight, int windowWidth)
        {
            return new Vector2(screenSize.X - windowWidth - WindowEdgeBuffer, screenSize.Y - windowHeight);
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

            if (LeftUnitSelectionWindow != null)
                LeftUnitSelectionWindow.Draw(spriteBatch,
                    LeftUnitSelectionWindowPosition(LeftUnitSelectionWindow.GetHeight()));

            if (RightUnitSelectionWindow != null)
                RightUnitSelectionWindow.Draw(spriteBatch,
                    RightUnitSelectionWindowPosition(RightUnitSelectionWindow.GetHeight(),
                        RightUnitSelectionWindow.GetWidth()));
        }
    }
}
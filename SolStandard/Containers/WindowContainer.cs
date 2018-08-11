using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;

namespace SolStandard.Containers
{
    public class WindowLayer : IGameLayer
    {
        public Window DebugWindow { get; set; }

        public Window LeftUnitSelectionWindow { get; set; }
        public Window RightUnitSelectionWindow { get; set; }

        public List<Window> ExtraWindows { get; set; }

        public WindowLayer(List<Window> extraWindows)
        {
            this.ExtraWindows = extraWindows;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Window window in ExtraWindows)
            {
                window.Draw(spriteBatch);
            }

            DebugWindow.Draw(spriteBatch);

            if (LeftUnitSelectionWindow != null)
                LeftUnitSelectionWindow.Draw(spriteBatch);

            if (RightUnitSelectionWindow != null)
                RightUnitSelectionWindow.Draw(spriteBatch);
        }
    }
}
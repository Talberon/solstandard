using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;

namespace SolStandard.Containers
{
    public class WindowLayer : IGameLayer
    {
        private List<Window> windows;

        public WindowLayer(List<Window> windows)
        {
            this.windows = windows;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Window window in windows)
            {
                window.Draw(spriteBatch);
            }
        }
    }
}
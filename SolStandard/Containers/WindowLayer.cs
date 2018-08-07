using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;

namespace SolStandard.Containers
{
    public class WindowLayer : IGameLayer
    {
        public List<Window> Windows { get; set; }
        
        public WindowLayer(List<Window> windows)
        {
            this.Windows = windows;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Window window in Windows)
            {
                window.Draw(spriteBatch);
            }
        }
    }
}
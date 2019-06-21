using Microsoft.Xna.Framework;

namespace SolStandard.HUD.Window.Animation
{
    public class WindowStatic : IWindowAnimation
    {
        public Vector2 CurrentPosition { get; private set; }

        public WindowStatic(Vector2 position)
        {
            CurrentPosition = position;
        }

        public void Update()
        {
            //Do nothing
        }
    }
}
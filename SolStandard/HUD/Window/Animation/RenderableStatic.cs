using Microsoft.Xna.Framework;

namespace SolStandard.HUD.Window.Animation
{
    public class RenderableStatic : IRenderableAnimation
    {
        public Vector2 CurrentPosition { get; private set; }

        public RenderableStatic(Vector2 position)
        {
            CurrentPosition = position;
        }

        public void Update(Vector2 destination)
        {
            CurrentPosition = destination;
        }
    }
}
using Microsoft.Xna.Framework;

namespace SolStandard.HUD.Window.Animation
{
    public interface IRenderableAnimation
    {
        void Update(Vector2 destination);
        Vector2 CurrentPosition { get; }
    }
}
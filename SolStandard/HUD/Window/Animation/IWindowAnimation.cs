using Microsoft.Xna.Framework;

namespace SolStandard.HUD.Window.Animation
{
    public interface IWindowAnimation
    {
        void Update(Vector2 destination);
        Vector2 CurrentPosition { get; }
    }
}
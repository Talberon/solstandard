using Microsoft.Xna.Framework;

namespace SolStandard.HUD.Window.Animation
{
    public enum WindowAnimation
    {
        None,
        Slide
    }

    public interface IWindowAnimation
    {
        void Update();
        Vector2 CurrentPosition { get; }
    }
}
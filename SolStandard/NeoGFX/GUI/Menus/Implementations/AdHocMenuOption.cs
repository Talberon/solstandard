using Microsoft.Xna.Framework;

namespace Steelbreakers.Utility.GUI.HUD.Menus.Implementations
{
    public class AdHocMenuOption : MenuOption
    {
        public AdHocMenuOption(OnConfirm onConfirm, OnHover onHover, OffHover offHover, Window.JuicyWindow juicyWindow)
            : base(onConfirm, onHover, offHover, juicyWindow)
        {
        }

        public AdHocMenuOption(OnConfirm onConfirm, OnHover onHover, OffHover offHover, string text, Color windowColor,
            float speed = 0.99f) : base(onConfirm, onHover, offHover, text, windowColor, speed)
        {
        }
    }
}
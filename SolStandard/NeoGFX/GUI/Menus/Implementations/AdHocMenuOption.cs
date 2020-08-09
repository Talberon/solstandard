using Microsoft.Xna.Framework;

namespace SolStandard.NeoGFX.GUI.Menus.Implementations
{
    public class AdHocMenuOption : MenuOption
    {
        public AdHocMenuOption(OnConfirm onConfirm, OnHover onHover, OffHover offHover, NeoWindow.JuicyWindow juicyWindow)
            : base(onConfirm, onHover, offHover, juicyWindow)
        {
        }

        public AdHocMenuOption(OnConfirm onConfirm, OnHover onHover, OffHover offHover, string text, Color windowColor,
            float speed = 0.99f) : base(onConfirm, onHover, offHover, text, windowColor, speed)
        {
        }
    }
}
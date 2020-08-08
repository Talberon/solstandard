using Microsoft.Xna.Framework;
using Steelbreakers.Utility.Monogame.Assets;

namespace Steelbreakers.Utility.GUI.HUD.Menus.Implementations
{
    public class UnselectableOption : MenuOption
    {
        public UnselectableOption(OnHover onHover, OffHover offHover, string text, Color windowColor) :
            base(PreventSelect(), onHover, offHover, text, windowColor)
        {
        }

        public static OnConfirm PreventSelect() => (option) =>
        {
            SoundBox.MenuError.Play();
            option.JuiceBox.ApplyTrauma(0.8f);
        };
    }
}
using Microsoft.Xna.Framework;
using SolStandard.NeoUtility.Monogame.Assets;

namespace SolStandard.NeoGFX.GUI.Menus.Implementations
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
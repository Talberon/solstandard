using Microsoft.Xna.Framework;
using SolStandard.NeoUtility.Monogame.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;

namespace SolStandard.NeoGFX.GUI.Menus
{
    public static class MenuOptionEffects
    {
        public static MenuOption.OnConfirm DoNothingConfirm => _ => { };
        public static MenuOption.OnHover DoNothingHover => _ => { };
        public static MenuOption.OffHover DoNothingUnhover => _ => { };

        public static MenuOption.OnHover HighlightOnHover(Color colorToSet) =>
            (option) => option.HueShiftToColor(colorToSet);

        public static MenuOption.OffHover HighlightOffHover(Color colorToSet) =>
            (option) => option.HueShiftToColor(colorToSet);

        public static MenuOption.OnHover DoubleWidth() => (option) => option.StretchWindowByFactor(2);
        public static MenuOption.OffHover HalveWidth() => (option) => option.ShrinkWindowByFactor(2);

        public static MenuOption.OnHover StretchAndFadeToColor(float stretchFactor, Color targetColor)
        {
            return (option) =>
            {
                option.StretchWindowByFactor(stretchFactor);
                option.HueShiftToColor(targetColor);
            };
        }

        public static MenuOption.OffHover CollapseAndFadeToColor(float destretchFactor, Color targetColor)
        {
            return (option) =>
            {
                option.ShrinkWindowByFactor(destretchFactor);
                option.HueShiftToColor(targetColor);
            };
        }

        public static void ClickOptionThenPerformAction(MenuOption option, AdHocEvent.EventAction action,
            int depressedFrames = 1, int frameDelayAfterClick = 0)
        {
            SoundBox.MenuConfirm.Play();

            Color originalColor = option.JuiceBox.CurrentColor;
            Color nextColor = Color.Lerp(originalColor, Color.Black, 0.3f);
            option.JuiceBox.HueSnapTo(nextColor);

            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(depressedFrames));
            GlobalEventQueue.QueueSingleEvent(new AdHocEvent(() => option.JuiceBox.HueSnapTo(originalColor)));
            if (frameDelayAfterClick > 0) GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(frameDelayAfterClick));
            GlobalEventQueue.QueueSingleEvent(new AdHocEvent(action));
        }

        //Helper Extensions

        private static void ShrinkWindowByFactor(this MenuOption option, float destretchFactor)
        {
            option.JuiceBox.ShiftToNewSize(new Vector2(
                option.JuiceBox.TargetSize.X / destretchFactor,
                option.JuiceBox.TargetSize.Y
            ));
        }

        private static void StretchWindowByFactor(this MenuOption option, float stretchFactor)
        {
            option.JuiceBox.ShiftToNewSize(new Vector2(
                option.JuiceBox.TargetSize.X * stretchFactor,
                option.JuiceBox.TargetSize.Y
            ));
        }

        private static void HueShiftToColor(this MenuOption option, Color targetColor)
        {
            option.JuiceBox.HueShiftTo(targetColor);
        }
    }
}
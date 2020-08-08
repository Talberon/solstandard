using SolStandard.NeoGFX.GUI;

namespace SolStandard.NeoGFX.Juice
{
    public static class JuiceBoxUtils
    {
        public static JuiceBox.Builder JuiceBoxForWindow(Window window, float speed)
        {
            return new JuiceBox.Builder(speed)
                .WithMoveSmoothing(window.CurrentPosition)
                .WithSizeSmoothing(window.Size())
                .WithColorShifting(window.DefaultColor);
        }

        public static Window.JuicyWindow ToJuicyWindow(this Window window, float speed = 0.99f)
        {
            return new Window.JuicyWindow(window, JuiceBoxForWindow(window, speed).Build());
        }
    }
}
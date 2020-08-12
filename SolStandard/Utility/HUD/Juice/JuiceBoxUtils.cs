using SolStandard.Utility.HUD.Neo;

namespace SolStandard.Utility.HUD.Juice
{
    public static class JuiceBoxUtils
    {
        public static JuiceBox.Builder JuiceBoxForWindow(NeoWindow window, float speed)
        {
            return new JuiceBox.Builder(speed)
                .WithMoveSmoothing(window.CurrentPosition)
                .WithSizeSmoothing(window.Size())
                .WithColorShifting(window.DefaultColor);
        }

        public static NeoWindow.JuicyWindow ToJuicyWindow(this NeoWindow window, float speed = 0.99f)
        {
            return new NeoWindow.JuicyWindow(window, JuiceBoxForWindow(window, speed).Build());
        }
    }
}
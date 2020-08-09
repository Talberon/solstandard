using SolStandard.NeoGFX.GUI;

namespace SolStandard.NeoGFX.Juice
{
    public static class JuiceBoxUtils
    {
        public static JuiceBox.Builder JuiceBoxForWindow(NeoWindow neoWindow, float speed)
        {
            return new JuiceBox.Builder(speed)
                .WithMoveSmoothing(neoWindow.CurrentPosition)
                .WithSizeSmoothing(neoWindow.Size())
                .WithColorShifting(neoWindow.DefaultColor);
        }

        public static NeoWindow.JuicyWindow ToJuicyWindow(this NeoWindow neoWindow, float speed = 0.99f)
        {
            return new NeoWindow.JuicyWindow(neoWindow, JuiceBoxForWindow(neoWindow, speed).Build());
        }
    }
}
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.HUD.Juice
{
    public class ColorShifter
    {
        public Color CurrentColor { get; private set; }
        public Color TargetColor { get; private set; }

        private readonly float speed;

        public ColorShifter(Color initialColor, float speed)
        {
            this.speed = speed;
            CurrentColor = initialColor;
            TargetColor = initialColor;
        }

        public void HueShiftTo(Color newTargetColor)
        {
            TargetColor = newTargetColor;
        }

        public void HueSnapTo(Color newTargetColor)
        {
            CurrentColor = newTargetColor;
            TargetColor = newTargetColor;
        }

        public void Update()
        {
            int nextRed = (int) MathUtils.AsymptoticAverage(CurrentColor.R, TargetColor.R, speed);
            int nextGreen = (int) MathUtils.AsymptoticAverage(CurrentColor.G, TargetColor.G, speed);
            int nextBlue = (int) MathUtils.AsymptoticAverage(CurrentColor.B, TargetColor.B, speed);
            int nextAlpha = (int) MathUtils.AsymptoticAverage(CurrentColor.A, TargetColor.A, speed);

            CurrentColor = new Color(nextRed, nextGreen, nextBlue, nextAlpha);
        }
    }
}
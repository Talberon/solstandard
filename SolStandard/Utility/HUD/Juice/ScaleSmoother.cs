namespace SolStandard.Utility.HUD.Juice
{
    public class ScaleSmoother
    {
        public float CurrentScale { get; private set; }
        public float TargetScale { get; private set; }

        private readonly float speed;

        public ScaleSmoother(float initialScale, float speed)
        {
            CurrentScale = initialScale;
            TargetScale = initialScale;
            this.speed = speed;
        }

        public void ShiftToNewScale(float nextSize)
        {
            TargetScale = nextSize;
        }

        public void SnapToNewScale(float nextSize)
        {
            CurrentScale = nextSize;
            TargetScale = nextSize;
        }

        public void Update()
        {
            CurrentScale = MathUtils.AsymptoticAverage(CurrentScale, TargetScale, speed);
        }
    }
}
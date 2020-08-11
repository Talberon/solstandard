using Microsoft.Xna.Framework;

namespace SolStandard.Utility.HUD.Juice
{
    public class SizeSmoother
    {
        public Vector2 CurrentSize { get; private set; }
        public Vector2 TargetSize { get; private set; }

        private readonly float speed;

        public SizeSmoother(Vector2 initialSize, float speed)
        {
            CurrentSize = initialSize;
            TargetSize = initialSize;
            this.speed = speed;
        }

        public void ShiftToNewSize(Vector2 nextSize)
        {
            TargetSize = nextSize;
        }

        public void SnapToNewSize(Vector2 nextSize)
        {
            CurrentSize = nextSize;
            TargetSize = nextSize;
        }

        public void Update()
        {
            float nextX = MathUtils.AsymptoticAverage(CurrentSize.X, TargetSize.X, speed);
            float nextY = MathUtils.AsymptoticAverage(CurrentSize.Y, TargetSize.Y, speed);
            CurrentSize = new Vector2(nextX, nextY);
        }
    }
}
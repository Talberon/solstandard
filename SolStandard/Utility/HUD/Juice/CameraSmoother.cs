using Microsoft.Xna.Framework;

namespace SolStandard.Utility.HUD.Juice
{
    public class CameraSmoother
    {
        public Vector2 CurrentPosition => moveSmoother.CurrentPosition;

        private float Speed { get; }

        public float CurrentZoom { get; private set; }
        private float targetZoom;

        private readonly MoveSmoother moveSmoother;

        public CameraSmoother(float speed, float startingZoom)
        {
            moveSmoother = new MoveSmoother(Vector2.Zero, speed);

            Speed = speed;
            CurrentZoom = startingZoom;
            targetZoom = CurrentZoom;
        }

        public void MoveTowards(Vector2 newTarget)
        {
            moveSmoother.MoveTowards(newTarget);
        }

        public void ZoomTowards(float newTarget)
        {
            targetZoom = newTarget;
        }

        public void Update()
        {
            moveSmoother.Update();
            CurrentZoom = MathUtils.AsymptoticAverage(CurrentZoom, targetZoom, Speed);
        }
    }
}
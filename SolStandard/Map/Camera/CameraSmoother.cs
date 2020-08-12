using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Map.Camera
{
    public class CameraSmoother
    {
        public Vector2 CurrentPosition => moveSmoother.CurrentPosition;
        public Vector2 TargetPosition => moveSmoother.TargetPosition;

        private float Speed { get; }

        public float CurrentZoom { get; private set; }
        public float TargetZoom { get; private set; }

        private readonly MoveSmoother moveSmoother;

        public CameraSmoother(float speed, float startingZoom)
        {
            moveSmoother = new MoveSmoother(Vector2.Zero, speed);

            Speed = speed;
            CurrentZoom = startingZoom;
            TargetZoom = CurrentZoom;
        }

        public void MoveTowards(Vector2 newTarget)
        {
            moveSmoother.MoveTowards(newTarget);
        }

        public void ZoomTowards(float newTarget)
        {
            TargetZoom = newTarget;
        }

        public void SnapMoveTo(Vector2 newTarget)
        {
            moveSmoother.SnapTo(newTarget);
        }

        public void SnapZoomTo(float newTarget)
        {
            TargetZoom = newTarget;
            CurrentZoom = TargetZoom;
        }

        public void MoveLeftSideTo(float x)
        {
            moveSmoother.MoveTowards(new Vector2(x, moveSmoother.TargetPosition.Y));
        }

        public void MoveRightSideTo(float x)
        {
            moveSmoother.MoveTowards(new Vector2(x - GameDriver.VirtualResolution.X, moveSmoother.TargetPosition.Y));
        }

        public void MoveTopSideTo(float y)
        {
            moveSmoother.MoveTowards(new Vector2(moveSmoother.TargetPosition.X, y));
        }

        public void MoveBottomSideTo(float y)
        {
            moveSmoother.MoveTowards(new Vector2(moveSmoother.TargetPosition.X, y - GameDriver.VirtualResolution.Y));
        }

        public void Update()
        {
            moveSmoother.Update();
            CurrentZoom = MathUtils.AsymptoticAverage(CurrentZoom, TargetZoom, Speed);
        }
    }
}
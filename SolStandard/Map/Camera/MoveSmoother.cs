using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Map.Camera
{
    public class MoveSmoother
    {
        public Vector2 CurrentPosition => currentPosition;
        public Vector2 TargetPosition => targetPosition;

        private float Speed { get; }

        private Vector2 currentPosition;
        private Vector2 targetPosition;

        public MoveSmoother(Vector2 initialPosition, float speed)
        {
            Speed = speed;
            currentPosition = initialPosition;
            targetPosition = initialPosition;
        }

        public void MoveTowards(Vector2 newTarget)
        {
            targetPosition = newTarget.WithoutPrecision();
        }

        public void SnapTo(Vector2 newTarget)
        {
            targetPosition = newTarget.WithoutPrecision();
            currentPosition = newTarget.WithoutPrecision();
        }

        public void Update()
        {
            currentPosition.X = MathUtils.AsymptoticAverage(currentPosition.X, targetPosition.X, Speed);
            currentPosition.Y = MathUtils.AsymptoticAverage(currentPosition.Y, targetPosition.Y, Speed);
        }
    }
}
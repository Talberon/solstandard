using Microsoft.Xna.Framework;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility.HUD.Juice
{
    public class JuiceBox
    {
        public Vector2 CurrentRealPosition => moveSmoother.CurrentPosition;
        public Vector2 TargetRealPosition => moveSmoother.TargetPosition;

        public Vector2 CurrentDrawPosition => shaker.ApplyShake(CurrentRealPosition);

        public Color CurrentColor => colorShifter.CurrentColor;
        public Color TargetColor => colorShifter.TargetColor;

        public Vector2 CurrentSize => boundsSmoother.CurrentSize;
        public Vector2 TargetSize => boundsSmoother.TargetSize;
        
        public Vector2 DefaultSize { get; set; }

        private readonly MoveSmoother moveSmoother;
        private readonly ColorShifter colorShifter;
        private readonly SizeSmoother boundsSmoother;
        private readonly Shaker shaker;

        private JuiceBox(
            Vector2 initialPosition,
            Color initialColor,
            float shakeTraumaDecayRate, float shakeMaxOffset,
            Vector2 initialSize,
            float speed
        )
        {
            moveSmoother = new MoveSmoother(initialPosition, speed);
            colorShifter = new ColorShifter(initialColor, speed);
            boundsSmoother = new SizeSmoother(initialSize, speed);
            shaker = new Shaker(shakeTraumaDecayRate, shakeMaxOffset);
            DefaultSize = initialSize;
        }

        public void ShiftTowards(Vector2 offset)
        {
            MoveTowards(moveSmoother.CurrentPosition + offset);
        }

        public void MoveTowards(Vector2 newTarget)
        {
            moveSmoother.MoveTowards(newTarget);
        }

        public void SnapTo(Vector2 newTarget)
        {
            moveSmoother.SnapTo(newTarget);
        }

        public void FadeTowards(float opacityBetweenZeroAndOne)
        {
            colorShifter.HueShiftTo(new Color(colorShifter.TargetColor, opacityBetweenZeroAndOne));
        }

        public void SetFadeTo(float opacityBetweenZeroAndOne)
        {
            colorShifter.HueSnapTo(new Color(colorShifter.TargetColor, opacityBetweenZeroAndOne));
        }

        public void HueShiftTo(Color newTargetColor)
        {
            colorShifter.HueShiftTo(newTargetColor);
        }

        public void HueSnapTo(Color newTargetColor)
        {
            colorShifter.HueSnapTo(newTargetColor);
        }

        public void ShiftToNewSize(Vector2 nextSize)
        {
            boundsSmoother.ShiftToNewSize(nextSize);
        }

        public void SnapToNewSize(Vector2 nextSize)
        {
            boundsSmoother.SnapToNewSize(nextSize);
        }

        public void ApplyTrauma(float betweenZeroAndOne)
        {
            shaker.ApplyTrauma(betweenZeroAndOne);
        }

        public Vector2 ApplyShake(Vector2 drawCoordinates)
        {
            return shaker.ApplyShake(drawCoordinates);
        }

        public void ResetTrauma()
        {
            shaker.ResetTrauma();
        }

        public void Update()
        {
            moveSmoother.Update();
            colorShifter.Update();
            boundsSmoother.Update();
            shaker.Update();
        }

        public class Builder
        {
            private Vector2 initialPosition;
            private Color initialColor;
            private float shakeTraumaDecayRate;
            private float shakeMaxOffset;
            private Vector2 initialSize;
            private readonly float speed;

            public Builder(float speedBetweenZeroAndOne)
            {
                initialPosition = Vector2.Zero;
                initialColor = Color.White;
                shakeTraumaDecayRate = 0.075f;
                shakeMaxOffset = 3f;
                initialSize = Vector2.Zero;
                speed = speedBetweenZeroAndOne;
            }

            public Builder WithMoveSmoothing(Vector2 startPosition)
            {
                initialPosition = startPosition;
                return this;
            }

            public Builder WithColorShifting(Color startColor)
            {
                initialColor = startColor;
                return this;
            }

            public Builder WithShaker(float traumaDecayRate, float maxOffset)
            {
                shakeTraumaDecayRate = traumaDecayRate;
                shakeMaxOffset = maxOffset;
                return this;
            }

            public Builder WithSizeSmoothing(Vector2 startSize)
            {
                initialSize = startSize;
                return this;
            }

            public JuiceBox Build()
            {
                if (speed <= 0f) throw new OutOfRangeException("Speed needs to be set above zero!");

                return new JuiceBox(
                    initialPosition,
                    initialColor,
                    shakeTraumaDecayRate,
                    shakeMaxOffset,
                    initialSize,
                    speed
                );
            }
        }
    }
}
using System;
using Microsoft.Xna.Framework;

namespace SolStandard.HUD.Window.Animation
{
    public class RenderableShake : IRenderableAnimation
    {
        private const float RadiusDecayRate = 0.6f;
        private float shakeRadius;
        private int shakeStartAngle;
        private readonly int durationInFrames;
        private bool isShaking;
        private int currentDuration;
        public Vector2 CurrentPosition { get; private set; }

        public RenderableShake(float maxOffset, int durationInFrames)
        {
            shakeRadius = maxOffset;
            this.durationInFrames = durationInFrames;
            currentDuration = 0;
            isShaking = true;
        }

        private Vector2 UpdateOffset(Vector2 origin)
        {
            if (!isShaking) return origin;

            Vector2 shakeOffset = origin +
                                  new Vector2(
                                      (float) (Math.Sin(shakeStartAngle) * shakeRadius),
                                      (float) (Math.Cos(shakeStartAngle) * shakeRadius)
                                  );
            shakeRadius -= RadiusDecayRate;
            shakeStartAngle += (160 + GameDriver.Random.Next(60));

            currentDuration++;
            if (currentDuration >= durationInFrames)
            {
                isShaking = false;
            }

            return shakeOffset;
        }

        public void Update(Vector2 destination)
        {
            CurrentPosition = UpdateOffset(destination);
        }
    }
}
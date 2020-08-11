using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SolStandard.Utility.HUD.Juice
{
    public class Shaker
    {
        private const float FloatTolerance = 0.001f;
        private readonly float traumaDecayRate;
        private Vector2 offset;
        private readonly float maxOffset;

        //Must stay between 0 and 1
        private float trauma;

        public Shaker(float traumaDecayRate, float maxOffset)
        {
            this.traumaDecayRate = traumaDecayRate;
            this.maxOffset = maxOffset;
            offset = Vector2.Zero;
        }

        public void ApplyTrauma(float betweenZeroAndOne)
        {
            if (betweenZeroAndOne < 0) betweenZeroAndOne = 0;
            if (betweenZeroAndOne + trauma > 1) betweenZeroAndOne = 1 - trauma;

            trauma += betweenZeroAndOne;
        }

        public void Update()
        {
            if (Math.Abs(trauma) < FloatTolerance || trauma - traumaDecayRate <= 0) trauma = 0;
            else trauma -= traumaDecayRate;
        }

        public Vector2 ApplyShake(Vector2 originalPoint)
        {
            offset.X = maxOffset * trauma * GetRandomFloatNegOneToOne();
            offset.Y = maxOffset * trauma * GetRandomFloatNegOneToOne();

            return originalPoint + offset;
        }

        public void ResetTrauma()
        {
            trauma = 0;
        }

        private static float GetRandomFloatNegOneToOne()
        {
            return GameDriver.Random.NextSingle(-1, 1);
        }
    }
}
using System;
using Microsoft.Xna.Framework;

namespace SolStandard.NeoUtility.General
{
    public class TimeSpanValueInverter<T>
    {
        private readonly T normalValue;
        private readonly T invertedValue;

        private TimeSpan TimeBetweenInversions { get; }
        private TimeSpan timeRemaining;
        private bool IsExpired => timeRemaining < TimeSpan.Zero;

        public T Value => IsInverted ? invertedValue : normalValue;
        public bool JustChanged { get; private set; }

        private bool IsInverted { get; set; }

        public TimeSpanValueInverter(T normalValue, T invertedValue, TimeSpan timeBetweenInversions)
        {
            this.normalValue = normalValue;
            this.invertedValue = invertedValue;
            TimeBetweenInversions = timeBetweenInversions;
            timeRemaining = TimeBetweenInversions;
            IsInverted = false;
            JustChanged = false;
        }

        public void Update(GameTime gameTime)
        {
            timeRemaining -= gameTime.ElapsedGameTime;
            
            JustChanged = false;

            if (!IsExpired) return;

            JustChanged = true;
            IsInverted = !IsInverted;
            timeRemaining = TimeBetweenInversions;
        }

        public void Reset()
        {
            timeRemaining = TimeBetweenInversions;
            IsInverted = false;
            JustChanged = true;
        }
    }
}
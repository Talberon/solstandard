using System;
using Microsoft.Xna.Framework;

namespace SolStandard.NeoUtility.General
{
    public class TimeSpanValueOverride<T>
    {
        public TimeSpan TimeRemaining { get; private set; }
        public bool JustChanged { get; private set; }

        public T Value => IsExpired ? originalValue : temporaryOverrideValue;

        private readonly T originalValue;
        private T temporaryOverrideValue;

        private bool IsExpired => TimeRemaining < TimeSpan.Zero;
        private bool wasExpired;

        public TimeSpanValueOverride(T originalValue)
        {
            this.originalValue = originalValue;
            temporaryOverrideValue = originalValue;
            TimeRemaining = TimeSpan.Zero;
            wasExpired = IsExpired;
        }

        public void OverrideForDuration(T newOverrideValue, TimeSpan duration)
        {
            temporaryOverrideValue = newOverrideValue;
            TimeRemaining = duration;
        }

        public void Reset()
        {
            TimeRemaining = TimeSpan.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsExpired) TimeRemaining -= gameTime.ElapsedGameTime;

            JustChanged = wasExpired != IsExpired;

            wasExpired = IsExpired;
        }
    }
}
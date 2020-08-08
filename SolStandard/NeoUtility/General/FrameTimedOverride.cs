namespace SolStandard.NeoUtility.General
{
    public class FrameTimedOverride<T>
    {
        public int FramesRemaining { get; private set; }

        private readonly T originalValue;
        private T temporaryOverrideValue;

        private bool IsExpired => FramesRemaining < 0;
        private bool wasExpired;

        public T Value => IsExpired ? originalValue : temporaryOverrideValue;

        public bool JustChanged { get; private set; }

        public FrameTimedOverride(T originalValue)
        {
            this.originalValue = originalValue;
            temporaryOverrideValue = originalValue;
            FramesRemaining = 0;
            wasExpired = IsExpired;
        }

        public void OverrideForFrames(T newOverrideValue, int frameDuration)
        {
            temporaryOverrideValue = newOverrideValue;
            FramesRemaining = frameDuration;
        }

        public void Reset()
        {
            FramesRemaining = 0;
        }

        public void Update()
        {
            if (!IsExpired) FramesRemaining--;

            JustChanged = wasExpired != IsExpired;

            wasExpired = IsExpired;
        }
    }
}
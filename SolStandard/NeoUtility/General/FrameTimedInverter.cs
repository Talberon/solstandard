namespace Steelbreakers.Utility.General
{
    public class FrameTimedInverter<T>
    {
        private T normalValue;
        private T invertedValue;

        private int framesBetweenInversions;
        private int framesRemaining;
        private bool IsExpired => framesRemaining <= 0;
        private bool isInverted;

        public T Value => isInverted ? invertedValue : normalValue;

        public FrameTimedInverter(T normalValue, T invertedValue, int framesBetweenInversions)
        {
            this.normalValue = normalValue;
            this.invertedValue = invertedValue;
            this.framesBetweenInversions = framesBetweenInversions;
            framesRemaining = 0;
            isInverted = false;
        }

        public void Update()
        {
            framesRemaining--;
            if (!IsExpired) return;


            isInverted = !isInverted;
            framesRemaining = framesBetweenInversions;
        }

        public void Reset()
        {
            framesRemaining = framesBetweenInversions;
            isInverted = false;
        }

        public void ResetWithPermanentInversion()
        {
            T newNormal = invertedValue;
            invertedValue = normalValue;
            normalValue = newNormal;
            Reset();
        }

        public void ResetWithNewInterval(int newIntervalInFrames)
        {
            Reset();
            framesBetweenInversions = newIntervalInFrames;
        }
    }
}
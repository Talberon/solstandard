namespace SolStandard.NeoUtility.General
{
    public class ManualInverter<T>
    {
        private readonly T normalValue;
        private readonly T invertedValue;
        private bool isInverted;
        public T Value => isInverted ? invertedValue : normalValue;

        public ManualInverter(T normalValue, T invertedValue)
        {
            this.normalValue = normalValue;
            this.invertedValue = invertedValue;
            isInverted = false;
        }

        public void Invert()
        {
            isInverted = !isInverted;
        }
    }
}
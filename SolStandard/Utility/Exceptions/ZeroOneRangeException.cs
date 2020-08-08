namespace SolStandard.Utility.Exceptions
{
    public class ZeroOneRangeException : OutOfRangeException
    {
        public ZeroOneRangeException(float value) : base($"Value {value} must be between zero and one inclusive!")
        {
        }

        public static void Assert(float valueMustBeBetweenZeroAndOne)
        {
            if (valueMustBeBetweenZeroAndOne < 0f || valueMustBeBetweenZeroAndOne > 1f)
            {
                throw new ZeroOneRangeException(valueMustBeBetweenZeroAndOne);
            }
        }
    }
}
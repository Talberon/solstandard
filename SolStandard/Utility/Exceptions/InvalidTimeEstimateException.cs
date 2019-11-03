using System;

namespace SolStandard.Utility.Exceptions
{
    [Serializable]
    internal class InvalidTimeEstimateException : Exception
    {
        public InvalidTimeEstimateException(int estimate) : base(
            $"Estimate out of range. Got {estimate} when it should be between 0 and 5 inclusively."
        )
        {
        }
    }
}
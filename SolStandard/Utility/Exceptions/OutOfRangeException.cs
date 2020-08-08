using System;

namespace SolStandard.Utility.Exceptions
{
    public class OutOfRangeException : Exception
    {
        public OutOfRangeException(string message) : base(message)
        {
        }
    }
}
using System;
using MonoGame.Extended;

namespace SolStandard.Utility.Exceptions
{
    [Serializable]
    public class InvalidShapeException : Exception
    {
        public InvalidShapeException(IShapeF shape) : base($"Invalid shape: {shape.GetType().FullName}")
        {
        }

        public InvalidShapeException(string message) : base(message)
        {
        }
    }
}
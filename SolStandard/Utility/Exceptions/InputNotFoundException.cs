using System;
using SolStandard.NeoUtility.Controls.Inputs;

namespace SolStandard.Utility.Exceptions
{
    public class InputNotFoundException : Exception
    {
        public InputNotFoundException(Input input, IController controller) : base(
            $"Input {input} could not be found because it wasn't mapped to controller: {controller}")
        {
        }
    }
}
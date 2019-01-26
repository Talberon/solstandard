using System.Collections.Generic;
using System.Linq;

namespace SolStandard.Utility.Buttons
{
    public class MultiControlParser : ControlMapper
    {
        private readonly List<ControlMapper> controlMappers;

        public MultiControlParser(params ControlMapper[] controlMappers)
        {
            this.controlMappers = controlMappers.ToList();
        }

        public override bool Press(Input input, PressType pressType)
        {
            foreach (ControlMapper controlMapper in controlMappers)
            {
                if (controlMapper.Press(input, pressType)) return true;
            }

            return false;
        }

        public override bool Peek(Input input, PressType pressType)
        {
            foreach (ControlMapper controlMapper in controlMappers)
            {
                if (controlMapper.Peek(input, pressType)) return true;
            }

            return false;
        }

        public override bool Released(Input input)
        {
            foreach (ControlMapper controlMapper in controlMappers)
            {
                if (controlMapper.Released(input)) return true;
            }

            return false;
        }
    }
}
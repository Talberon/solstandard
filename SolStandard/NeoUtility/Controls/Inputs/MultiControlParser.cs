using System.Collections.Generic;
using System.Linq;

namespace SolStandard.NeoUtility.Controls.Inputs
{
    public class MultiControlParser : ControlMapper
    {
        private readonly List<ControlMapper> controlMappers;

        public MultiControlParser(params ControlMapper[] controlMappers) : base(controlMappers.First().Controller)
        {
            this.controlMappers = controlMappers.ToList();
        }

        public override bool Press(Input input, PressType pressType)
        {
            return controlMappers.Any(controlMapper => controlMapper.Press(input, pressType));
        }

        public override bool Peek(Input input, PressType pressType)
        {
            return controlMappers.Any(controlMapper => controlMapper.Peek(input, pressType));
        }

        public override bool JustReleased(Input input)
        {
            return controlMappers.Any(controlMapper => controlMapper.JustReleased(input));
        }
    }
}
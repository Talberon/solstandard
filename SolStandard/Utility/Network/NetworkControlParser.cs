using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Network;

namespace SolStandard.Utility.Network
{
    public class NetworkControlParser : ControlMapper
    {
        private readonly NetworkController controller;

        public NetworkControlParser(NetworkController controller) : base (ControlType.Keyboard)
        {
            this.controller = controller;
        }

        public override bool Press(Input input, PressType pressType)
        {
            return controller.GetInput(input).Pressed;
        }

        public override bool Peek(Input input, PressType pressType)
        {
            return Press(input, pressType);
        }

        public override bool Released(Input input)
        {
            return controller.GetInput(input).Released;
        }
    }
}
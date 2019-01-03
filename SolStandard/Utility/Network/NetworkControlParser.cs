using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Network;

namespace SolStandard.Utility.Network
{
    public class NetworkControlParser : ControlMapper
    {
        private readonly NetworkController controller;

        public NetworkControlParser(NetworkController controller)
        {
            this.controller = controller;
        }

        public override bool Press(Input input, PressType pressType)
        {
            //TODO Figure out how pressType will be handled
            return controller.GetInput(input).Pressed;
        }

        public override bool Released(Input input)
        {
            return controller.GetInput(input).Released;
        }
    }
}
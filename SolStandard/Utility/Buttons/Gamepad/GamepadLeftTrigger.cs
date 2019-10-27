using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeftTrigger : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Lt;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Triggers.Left > ControlMapper.TriggerDeadzone;

        public GamepadLeftTrigger(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadLeftTrigger;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
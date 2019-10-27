using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRightTrigger : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Rt;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Triggers.Right > ControlMapper.TriggerDeadzone;

        public GamepadRightTrigger(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRightTrigger;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
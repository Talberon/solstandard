using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRightBumper : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Rb;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.RightShoulder == ButtonState.Pressed;

        public GamepadRightBumper(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRightBumper;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
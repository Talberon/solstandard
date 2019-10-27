using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadB : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.B;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.B == ButtonState.Pressed;

        public GamepadB(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadB;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
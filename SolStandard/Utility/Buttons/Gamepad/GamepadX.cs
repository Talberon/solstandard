using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadX : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.X;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.X == ButtonState.Pressed;

        public GamepadX(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadX;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
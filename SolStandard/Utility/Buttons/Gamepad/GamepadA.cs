using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadA : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.A;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.A == ButtonState.Pressed;

        public GamepadA(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadA;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
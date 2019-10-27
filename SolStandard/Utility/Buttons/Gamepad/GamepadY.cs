using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadY : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Y;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.Y == ButtonState.Pressed;

        public GamepadY(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadY;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
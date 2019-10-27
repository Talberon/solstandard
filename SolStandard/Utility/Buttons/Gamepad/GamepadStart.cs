using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadStart : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Start;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.Start == ButtonState.Pressed;

        public GamepadStart(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadStart;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
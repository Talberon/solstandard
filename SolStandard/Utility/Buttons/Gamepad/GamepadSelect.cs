using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadSelect : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Select;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.Back == ButtonState.Pressed;

        public GamepadSelect(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadSelect;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
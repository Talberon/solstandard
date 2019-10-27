using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeftBumper : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Lb;
        public override bool Pressed => GamePad.GetState(PlayerIndex).Buttons.LeftShoulder == ButtonState.Pressed;

        public GamepadLeftBumper(PlayerIndex playerIndex) : base(playerIndex)
        {
        }


        public override bool Equals(object obj)
        {
            return obj is GamepadLeftBumper;
        }


        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
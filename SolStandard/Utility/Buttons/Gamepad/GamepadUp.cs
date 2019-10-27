using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadUp : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Up;

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).DPad.Up == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y > ControlMapper.StickDeadzone;

        public GamepadUp(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadUp;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
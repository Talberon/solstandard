using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsDown : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.RightStickDown;

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).ThumbSticks.Right.Y < (-ControlMapper.StickDeadzone);

        public GamepadRsDown(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRsDown;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
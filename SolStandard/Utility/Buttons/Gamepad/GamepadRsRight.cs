using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsRight : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.RightStickRight;

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).ThumbSticks.Right.X > (ControlMapper.StickDeadzone);

        public GamepadRsRight(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRsRight;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
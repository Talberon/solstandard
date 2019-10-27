using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsUp : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.RightStickUp;
        public override bool Pressed => GamePad.GetState(PlayerIndex).ThumbSticks.Right.Y > ControlMapper.StickDeadzone;

        public GamepadRsUp(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadRsUp;
        }


        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsLeft : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.RightStickLeft;

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).ThumbSticks.Right.X < (-ControlMapper.StickDeadzone);

        public GamepadRsLeft(PlayerIndex playerIndex) : base(playerIndex)
        {
        }


        public override bool Equals(object obj)
        {
            return obj is GamepadRsLeft;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
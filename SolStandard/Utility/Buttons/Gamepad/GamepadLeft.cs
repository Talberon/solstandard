using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeft : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Left;

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).DPad.Left == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex).ThumbSticks.Left.X < (-ControlMapper.StickDeadzone);

        public GamepadLeft(PlayerIndex playerIndex) : base(playerIndex)
        {
        }


        public override bool Equals(object obj)
        {
            return obj is GamepadLeft;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
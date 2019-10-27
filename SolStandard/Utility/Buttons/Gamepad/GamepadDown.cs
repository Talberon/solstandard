using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadDown : GamePadControl
    {
        public override GamepadInputs InputType => GamepadInputs.Down;

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).DPad.Down == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y < (-ControlMapper.StickDeadzone);

        public GamepadDown(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is GamepadDown;
        }

        public override int GetHashCode()
        {
            return (int) PlayerIndex * (int) InputType;
        }
    }
}
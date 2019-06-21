using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRight: GamePadControl
    {
        public GamepadRight(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed =>
            GamePad.GetState(PlayerIndex).DPad.Right == ButtonState.Pressed ||
            GamePad.GetState(PlayerIndex).ThumbSticks.Left.X > (ControlMapper.StickDeadzone);
    }
}
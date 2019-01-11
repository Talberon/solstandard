using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeft: GamePadControl
    {
        public GamepadLeft(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Left == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.X < (-ControlMapper.StickDeadzone);
            }
        }
    }
}
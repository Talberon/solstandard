using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadUp : GameControl
    {
        public GamepadUp(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Up == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y > ControlMapper.StickDeadzone;
            }
        }
    }
}
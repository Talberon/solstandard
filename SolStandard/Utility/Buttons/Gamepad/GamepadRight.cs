using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRight : GameControl
    {
        public GamepadRight(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Right == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.X > (GameControlMapper.StickThreshold);
            }
        }
    }
}
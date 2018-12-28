using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadDown : GameControl
    {
        public GamepadDown(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Down == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y < (-GameControlMapper.StickThreshold);
            }
        }
    }
}
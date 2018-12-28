using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadLeft : GameControl
    {
        public GamepadLeft(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Left == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.X < (-GameControlMapper.StickThreshold);
            }
        }
    }
}
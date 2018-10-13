using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class LeftControl : GameControl
    {
        public LeftControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Left == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.X < (-GameControlMapper.StickThreshold) ||
                       Keyboard.GetState().IsKeyDown(Keys.A);
            }
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RightControl : GameControl
    {
        public RightControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Right == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.X > (GameControlMapper.StickThreshold) ||
                       Keyboard.GetState().IsKeyDown(Keys.D);
            }
        }
    }
}
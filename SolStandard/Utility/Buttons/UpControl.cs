using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class UpControl : GameControl
    {
        public UpControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Up == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y > GameControlMapper.StickThreshold ||
                       Keyboard.GetState().IsKeyDown(Keys.W);
            }
        }
    }
}
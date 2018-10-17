using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class DownControl : GameControl
    {
        public DownControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).DPad.Down == ButtonState.Pressed ||
                       GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y < (-GameControlMapper.StickThreshold) ||
                       Keyboard.GetState().IsKeyDown(Keys.S);
            }
        }
    }
}
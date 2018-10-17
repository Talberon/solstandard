using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RsRightControl : GameControl
    {
        public RsRightControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).ThumbSticks.Right.X > (GameControlMapper.StickThreshold) ||
                       Keyboard.GetState().IsKeyDown(Keys.Right);
            }
        }
    }
}
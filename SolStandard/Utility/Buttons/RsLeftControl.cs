using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RsLeftControl : GameControl
    {
        public RsLeftControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }
        
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).ThumbSticks.Right.X < (-GameControlMapper.StickThreshold) ||
                   Keyboard.GetState().IsKeyDown(Keys.Left);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).ThumbSticks.Right.X > (-GameControlMapper.StickThreshold) &&
                   Keyboard.GetState().IsKeyUp(Keys.Left);
        }
    }
}
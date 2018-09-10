using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RsUpControl : GameControl
    {
        public RsUpControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }
        
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).ThumbSticks.Right.Y > GameControlMapper.StickThreshold ||
                   Keyboard.GetState().IsKeyDown(Keys.Up);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).ThumbSticks.Right.Y < GameControlMapper.StickThreshold &&
                   Keyboard.GetState().IsKeyUp(Keys.Up);
        }
    }
}
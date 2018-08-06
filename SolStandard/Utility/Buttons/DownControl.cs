using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class DownControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed ||
                   GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < (-GameControlMapper.StickThreshold) ||
                   Keyboard.GetState().IsKeyDown(Keys.S);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Released &&
                   GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > (-GameControlMapper.StickThreshold) &&
                   Keyboard.GetState().IsKeyUp(Keys.S);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class LeftControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed ||
                   GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < (-GameControlMapper.StickThreshold) ||
                   Keyboard.GetState().IsKeyDown(Keys.A);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Released &&
                   GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > (-GameControlMapper.StickThreshold) &&
                   Keyboard.GetState().IsKeyUp(Keys.A);
        }
    }
}
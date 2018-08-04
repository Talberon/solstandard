using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RightControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed ||
                   GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > (GameControlMapper.StickThreshold) ||
                   Keyboard.GetState().IsKeyDown(Keys.D);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Released &&
                   GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < (GameControlMapper.StickThreshold) &&
                   Keyboard.GetState().IsKeyUp(Keys.D);
        }
    }
}
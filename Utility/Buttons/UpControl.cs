using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class UpControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed ||
                   GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > GameControlMapper.StickThreshold ||
                   Keyboard.GetState().IsKeyDown(Keys.W);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Released &&
                   GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < GameControlMapper.StickThreshold &&
                   Keyboard.GetState().IsKeyUp(Keys.W);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RightControl : GameControl
    {
        public RightControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).DPad.Right == ButtonState.Pressed ||
                   GamePad.GetState(PlayerIndex).ThumbSticks.Left.X > (GameControlMapper.StickThreshold) ||
                   Keyboard.GetState().IsKeyDown(Keys.D);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).DPad.Right == ButtonState.Released &&
                   GamePad.GetState(PlayerIndex).ThumbSticks.Left.X < (GameControlMapper.StickThreshold) &&
                   Keyboard.GetState().IsKeyUp(Keys.D);
        }
    }
}
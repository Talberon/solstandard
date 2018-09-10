using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class DownControl : GameControl
    {
        public DownControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).DPad.Down == ButtonState.Pressed ||
                   GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y < (-GameControlMapper.StickThreshold) ||
                   Keyboard.GetState().IsKeyDown(Keys.S);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).DPad.Down == ButtonState.Released &&
                   GamePad.GetState(PlayerIndex).ThumbSticks.Left.Y > (-GameControlMapper.StickThreshold) &&
                   Keyboard.GetState().IsKeyUp(Keys.S);
        }
    }
}
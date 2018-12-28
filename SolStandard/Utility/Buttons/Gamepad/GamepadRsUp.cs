using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsUp : GameControl
    {
        public GamepadRsUp(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get { return GamePad.GetState(PlayerIndex).ThumbSticks.Right.Y > GameControlMapper.StickThreshold; }
        }
    }
}
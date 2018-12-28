using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadRsDown : GameControl
    {
        public GamepadRsDown(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get { return GamePad.GetState(PlayerIndex).ThumbSticks.Right.Y < (-ControlMapper.StickDeadzone); }
        }
    }
}
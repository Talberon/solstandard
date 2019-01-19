using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadStart: GamePadControl
    {
        public GamepadStart(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get { return GamePad.GetState(PlayerIndex).Buttons.Start == ButtonState.Pressed; }
        }
    }
}
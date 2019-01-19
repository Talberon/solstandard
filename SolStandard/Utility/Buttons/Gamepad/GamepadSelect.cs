using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadSelect: GamePadControl
    {
        public GamepadSelect(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get { return GamePad.GetState(PlayerIndex).Buttons.Back == ButtonState.Pressed; }
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class BControl : GameControl
    {
        public BControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).Buttons.B == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.LeftShift);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).Buttons.B == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.LeftShift);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class BControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.LeftShift);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.LeftShift);
        }
    }
}
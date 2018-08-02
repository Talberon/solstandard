using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class XControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.Tab);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.Tab);
        }
    }
}
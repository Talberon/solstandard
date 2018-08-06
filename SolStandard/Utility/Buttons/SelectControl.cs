using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class SelectControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.Escape);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.Escape);
        }
    }
}
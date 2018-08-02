using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class StartControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.Enter);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.Enter);
        }
    }
}
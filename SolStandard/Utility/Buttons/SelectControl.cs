using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class SelectControl : GameControl
    {
        public SelectControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).Buttons.Back == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.Escape);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).Buttons.Back == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.Escape);
        }
    }
}
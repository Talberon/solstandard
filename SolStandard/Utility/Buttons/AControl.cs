using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class AControl : GameControl
    {
        public AControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).Buttons.A == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.Space);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).Buttons.A == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.Space);
        }
    }
}
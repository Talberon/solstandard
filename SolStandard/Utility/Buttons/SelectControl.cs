using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class SelectControl : GameControl
    {
        public SelectControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).Buttons.Back == ButtonState.Pressed ||
                       Keyboard.GetState().IsKeyDown(Keys.Escape);
            }
        }
    }
}
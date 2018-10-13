using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class XControl : GameControl
    {
        public XControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).Buttons.X == ButtonState.Pressed ||
                       Keyboard.GetState().IsKeyDown(Keys.Tab);
            }
        }
    }
}
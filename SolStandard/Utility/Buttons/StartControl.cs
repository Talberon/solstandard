using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class StartControl : GameControl
    {
        public StartControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).Buttons.Start == ButtonState.Pressed ||
                       Keyboard.GetState().IsKeyDown(Keys.Enter);
            }
        }
    }
}
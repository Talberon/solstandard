using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class StartControl : GameControl
    {
        public StartControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }
        
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).Buttons.Start == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.Enter);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).Buttons.Start == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.Enter);
        }
    }
}
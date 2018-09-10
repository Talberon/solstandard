using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class YControl : GameControl
    {
        public YControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }


        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).Buttons.Y == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.OemTilde);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).Buttons.Y == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.OemTilde);
        }
    }
}
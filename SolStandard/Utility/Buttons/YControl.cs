using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class YControl : GameControl
    {
        public YControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).Buttons.Y == ButtonState.Pressed ||
                       Keyboard.GetState().IsKeyDown(Keys.OemTilde);
            }
        }
    }
}
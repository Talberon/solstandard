using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class LeftBumperControl : GameControl
    {
        public LeftBumperControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).Buttons.LeftShoulder == ButtonState.Pressed ||
                       Keyboard.GetState().IsKeyDown(Keys.LeftControl);
            }
        }
    }
}
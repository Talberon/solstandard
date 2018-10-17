using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RightBumperControl : GameControl
    {
        public RightBumperControl(PlayerIndex playerIndex) : base(playerIndex)
        {
        }

        public override bool Pressed
        {
            get
            {
                return GamePad.GetState(PlayerIndex).Buttons.RightShoulder == ButtonState.Pressed ||
                       Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
            }
        }
    }
}
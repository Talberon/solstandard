using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class LeftBumperControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.LeftControl);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.LeftControl);
        }
    }
}
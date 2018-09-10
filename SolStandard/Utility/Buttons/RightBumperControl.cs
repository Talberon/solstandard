using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RightBumperControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed ||
                   Keyboard.GetState().IsKeyDown(Keys.LeftAlt);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Released &&
                   Keyboard.GetState().IsKeyUp(Keys.LeftAlt);
        }
    }
}
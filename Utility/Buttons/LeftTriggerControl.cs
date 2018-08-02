using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class LeftTriggerControl : GameControl
    {
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex.One).Triggers.Left > 0.2f || Keyboard.GetState().IsKeyDown(Keys.Q);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex.One).Triggers.Left == 0f && Keyboard.GetState().IsKeyUp(Keys.Q);
        }
    }
}
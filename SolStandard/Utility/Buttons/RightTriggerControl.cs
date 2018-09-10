using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class RightTriggerControl : GameControl
    {
        public RightTriggerControl(PlayerIndex playerIndex) : base (playerIndex)
        {
        } 
        
        public override bool Pressed()
        {
            return GamePad.GetState(PlayerIndex).Triggers.Right > 0.2f || Keyboard.GetState().IsKeyDown(Keys.E);
        }

        public override bool Released()
        {
            return GamePad.GetState(PlayerIndex).Triggers.Right < 0.01f && Keyboard.GetState().IsKeyUp(Keys.E);
        }
    }
}
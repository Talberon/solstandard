using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Buttons.KeyboardInput
{
    public class InputKey : GameControl
    {
        private readonly Keys key;

        public InputKey(PlayerIndex playerIndex, Keys key) : base(playerIndex)
        {
            this.key = key;
        }

        public override bool Pressed
        {
            get { return Keyboard.GetState().IsKeyDown(key); }
        }
    }
}
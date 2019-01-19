using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.KeyboardInput
{
    public class InputKey : GameControl
    {
        private readonly Keys key;

        public InputKey(Keys key)
        {
            this.key = key;
        }

        public override bool Pressed
        {
            get { return Keyboard.GetState().IsKeyDown(key); }
        }
    }
}
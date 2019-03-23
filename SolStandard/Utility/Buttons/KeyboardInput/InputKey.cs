using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons.KeyboardInput
{
    public class InputKey : GameControl
    {
        public readonly Keys Key;

        public InputKey(Keys key)
        {
            Key = key;
        }

        public override bool Pressed
        {
            get { return Keyboard.GetState().IsKeyDown(Key); }
        }
    }
}
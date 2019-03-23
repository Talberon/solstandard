using Microsoft.Xna.Framework;
using SolStandard.Utility;
using SolStandard.Utility.Buttons.KeyboardInput;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class KeyboardOption : ControlOption
    {
        private readonly KeyboardController keyboard;

        public KeyboardOption(KeyboardController keyboard, Color optionColor) :
            base(BuildGamepadMappingWindow(keyboard, optionColor), optionColor)
        {
            this.keyboard = keyboard;
        }

        public override void Execute()
        {
            //Do nothing
        }

        public override IRenderable Clone()
        {
            return new KeyboardOption(keyboard, DefaultColor);
        }
    }
}
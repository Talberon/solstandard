using Microsoft.Xna.Framework;
using SolStandard.HUD.Window;
using SolStandard.Utility;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class GamepadOption : AllControlsOption
    {
        private readonly GamepadController gamepad;

        public GamepadOption(GamepadController gamepad, Color optionColor) :
            base(BuildGamepadMappingWindow(gamepad, optionColor), optionColor, HorizontalAlignment.Centered)
        {
            this.gamepad = gamepad;
        }

        public override void Execute()
        {
            //Do nothing
        }

        public override IRenderable Clone()
        {
            return new GamepadOption(gamepad, DefaultColor);
        }
    }
}
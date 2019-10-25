using Microsoft.Xna.Framework;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class InputConfigOption : MenuOption
    {
        private readonly IController controller;
        private readonly Input input;

        public InputConfigOption(IController controller, Input input, Color color) :
            base(GenerateLabelContent(controller, input, color), color, HorizontalAlignment.Right)
        {
            this.controller = controller;
            this.input = input;
        }

        private static IRenderable GenerateLabelContent(IController controller, Input input, Color color)
        {
            return new Window.Window(
                new[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont, input.ToString().ToUpper()),
                        controller.GetInput(input).GetInputIcon(GameDriver.CellSize)
                    }
                },
                color
            );
        }

        public override void Execute()
        {
            //TODO Pop-up modal to set input

            //TODO Remove SFX
            AssetManager.CombatDeathSFX.Play();
        }

        public void SetNewInput(GameControl newInput)
        {
            controller.RemapControl(input, newInput);
            LabelContent = GenerateLabelContent(controller, input, DefaultColor);
        }

        public override IRenderable Clone()
        {
            return new InputConfigOption(controller, input, DefaultColor);
        }
    }
}
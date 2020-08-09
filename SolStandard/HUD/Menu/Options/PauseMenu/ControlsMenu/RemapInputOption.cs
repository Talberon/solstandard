using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.InputRemapping;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class RemapInputOption : MenuOption
    {
        private readonly IController controller;
        private readonly ControlConfigContext.Device device;
        private Input Input { get; }

        public RemapInputOption(IController controller, Input input, ControlConfigContext.Device device, Color color) :
            base(GenerateLabelContent(controller, input, color), color, HorizontalAlignment.Right)
        {
            this.controller = controller;
            Input = input;
            this.device = device;
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
            GlobalContext.ControlConfigContext.StartListeningForInput(device, Input);
        }

        public override IRenderable Clone()
        {
            return new RemapInputOption(controller, Input, device, DefaultColor);
        }
    }
}
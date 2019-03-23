using System;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public abstract class ControlOption : UnselectableOption
    {
        protected ControlOption(IRenderable labelContent, Color color,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left) :
            base(labelContent, color, horizontalAlignment)
        {
        }

        protected static Window.Window BuildGamepadMappingWindow(IController gamepad, Color windowColor)
        {
            Vector2 iconSize = new Vector2(GameDriver.CellSize);
            int inputCount = Enum.GetNames(typeof(Input)).Length;

            IRenderable[,] buttonGrid = new IRenderable[inputCount, 2];

            int gridRow = 0;
            foreach (Input input in (Input[]) Enum.GetValues(typeof(Input)))
            {
                buttonGrid[gridRow, 0] = new RenderText(AssetManager.WindowFont, input.ToString().ToUpper());
                buttonGrid[gridRow, 1] = gamepad.GetInputIcon(input, iconSize);

                gridRow++;
            }

            WindowContentGrid contentGrid = new WindowContentGrid(buttonGrid, 3, HorizontalAlignment.Right);

            return new Window.Window(contentGrid, windowColor, HorizontalAlignment.Centered);
        }
    }
}
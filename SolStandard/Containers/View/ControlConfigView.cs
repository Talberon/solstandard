using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Buttons;

namespace SolStandard.Containers.View
{
    public class ControlConfigView : IUserInterface
    {
        private IMenu ActiveMenu { get; }

        private IMenu TopLevelConfigMenu;


        private static IMenu GenerateControlMenu(IRenderable cursorSprite, Color windowColor)
        {
            Color keyboardOptionColor = TeamUtility.DetermineTeamColor(Team.Creep);
            Color playerOneColor = TeamUtility.DetermineTeamColor(GameContext.P1Team);
            Color playerTwoColor = TeamUtility.DetermineTeamColor(GameContext.P2Team);

            int inputs = Enum.GetValues(typeof(Input)).Length;
            MenuOption[] keyboardOptions = new MenuOption[inputs];
            MenuOption[] gamepadOneOptions = new MenuOption[inputs];
            MenuOption[] gamepadTwoOptions = new MenuOption[inputs];

            for (int i = 0; i < inputs; i++)
            {
                keyboardOptions[i] =
                    new RemapInputOption(GameDriver.KeyboardParser.Controller, (Input) i, keyboardOptionColor);
                gamepadOneOptions[i] =
                    new RemapInputOption(GameDriver.P1GamepadParser.Controller, (Input) i, playerOneColor);
                gamepadTwoOptions[i] =
                    new RemapInputOption(GameDriver.P2GamepadParser.Controller, (Input) i, playerTwoColor);
            }

            TwoDimensionalMenu menu = new TwoDimensionalMenu(
                new MenuOption[,]
                {
                    {
                        new SubmenuOption(
                            new VerticalMenu(keyboardOptions, cursorSprite, keyboardOptionColor),
                            RenderBlank.Blank,
                            "Keyboard Config",
                            windowColor
                        ),
                        new SubmenuOption(
                            new VerticalMenu(gamepadOneOptions, cursorSprite, playerOneColor),
                            RenderBlank.Blank,
                            "Gamepad 1 Config",
                            windowColor
                        ),
                        new SubmenuOption(
                            new VerticalMenu(gamepadTwoOptions, cursorSprite, playerTwoColor),
                            RenderBlank.Blank,
                            "Gamepad 2 Config",
                            windowColor
                        )
                    }
                },
                cursorSprite,
                windowColor,
                TwoDimensionalMenu.CursorType.Pointer
            );

            return menu;
        }

        public void ToggleVisible()
        {
            throw new System.NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;

namespace SolStandard.HUD.Menu
{
    public class VerticalMenu : IMenu
    {
        private const int ButtonIconSize = 32;
        private const int Padding = 2;

        private readonly MenuOption[] options;
        private readonly Dictionary<MenuOption, Vector2> optionCoordinates;
        private readonly Window.Window menuWindow;
        private readonly IRenderable cursorSprite;
        private Vector2 cursorPosition;

        private int CurrentOptionIndex { get; set; }
        public Color DefaultColor { get; set; }
        public bool IsVisible { get; set; }

        private static IRenderable ConfirmButton =>
            InputIconProvider.GetInputIcon(Input.Confirm, ButtonIconSize);

        public VerticalMenu(MenuOption[] options, IRenderable cursorSprite, Color color)
        {
            this.options = options;
            this.cursorSprite = cursorSprite;
            DefaultColor = color;
            CurrentOptionIndex = 0;
            cursorPosition = Vector2.Zero;
            optionCoordinates = MapOptionCoordinates();
            PositionCursorToOption();
            menuWindow = BuildMenuWindow();
            IsVisible = true;
        }

        private Window.Window BuildMenuWindow()
        {
            ResizeOptionsToWidestWidth(options);

            var optionWindows = new IRenderable[options.Length, 1];

            for (int i = 0; i < options.Length; i++)
            {
                optionWindows[i, 0] = options[i];
            }

            var optionsContent = new WindowContentGrid(optionWindows, Padding);

            return new Window.Window(optionsContent, DefaultColor);
        }

        private static void ResizeOptionsToWidestWidth(MenuOption[] options)
        {
            int widestWidth = 0;

            foreach (MenuOption option in options)
            {
                if (widestWidth < option.Width)
                {
                    widestWidth = option.Width;
                }
            }

            foreach (MenuOption menuOption in options)
            {
                menuOption.Width = widestWidth;
            }
        }

        public int Height => menuWindow.Height;


        public int Width => menuWindow.Width;

        private Dictionary<MenuOption, Vector2> MapOptionCoordinates()
        {
            var optionCoordinatesMapping = new Dictionary<MenuOption, Vector2>();

            Vector2 lastCoordinates =
                new Vector2(AssetManager.WindowTexture.Width, AssetManager.WindowTexture.Height) / 3;

            foreach (MenuOption option in options)
            {
                optionCoordinatesMapping[option] = lastCoordinates;
                lastCoordinates.Y += option.Height + Padding;
            }

            return optionCoordinatesMapping;
        }


        public MenuOption CurrentOption => options[CurrentOptionIndex];

        public void SelectOption()
        {
            CurrentOption.Execute();
            AssetManager.MenuConfirmSFX.Play();
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
        {
            switch (direction)
            {
                case MenuCursorDirection.Down:
                    if (CurrentOptionIndex < options.Length - 1)
                    {
                        CurrentOptionIndex++;
                    }
                    else
                    {
                        CurrentOptionIndex = 0;
                    }

                    PositionCursorToOption();
                    AssetManager.MenuMoveSFX.Play();
                    break;
                case MenuCursorDirection.Up:
                    if (CurrentOptionIndex > 0)
                    {
                        CurrentOptionIndex--;
                    }
                    else
                    {
                        CurrentOptionIndex = options.Length - 1;
                    }

                    PositionCursorToOption();
                    AssetManager.MenuMoveSFX.Play();

                    break;
                default:
                    //Do nothing
                    return;
            }
        }

        private void PositionCursorToOption()
        {
            MenuOption currentOption = options[CurrentOptionIndex];

            (float x, float y) = optionCoordinates[currentOption];

            y += (float) currentOption.Height / 2;

            cursorPosition = new Vector2(
                x - cursorSprite.Width,
                y - (float) cursorSprite.Height / 2
            );
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            if (!IsVisible) return;
            menuWindow.Draw(spriteBatch, position, colorOverride);
            
            Color cursorColor = TeamUtility.DetermineTeamCursorColor(GlobalContext.ActiveTeam);
            cursorSprite.Draw(spriteBatch, position + cursorPosition, cursorColor);

            ConfirmButton.Draw(spriteBatch,
                position +
                cursorPosition +
                TwoDimensionalMenu.CenterLeftOffset(ConfirmButton, cursorSprite) + TwoDimensionalMenu.IconOffsetHack
            );
        }

        public IRenderable Clone()
        {
            return new VerticalMenu(options, cursorSprite, DefaultColor);
        }
    }
}
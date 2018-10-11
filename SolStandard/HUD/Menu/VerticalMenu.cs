using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu
{
    public class VerticalMenu : IRenderable
    {
        public enum MenuCursorDirection
        {
            Forward,
            Backward
        }

        private readonly IRenderable cursorSprite;
        private Vector2 cursorPosition;
        private readonly MenuOption[] options;
        private readonly Dictionary<MenuOption, Vector2> optionCoordinates;
        public int CurrentOptionIndex { get; private set; }
        private readonly Color backgroundColor;
        private const int Padding = 2;
        private readonly Window.Window menuWindow;

        public VerticalMenu(MenuOption[] options, IRenderable cursorSprite, Color backgroundColor)
        {
            this.options = options;
            this.cursorSprite = cursorSprite;
            this.backgroundColor = backgroundColor;
            CurrentOptionIndex = 0;
            cursorPosition = Vector2.Zero;
            optionCoordinates = MapOptionCoordinates();
            PositionCursorToOption();
            menuWindow = BuildMenuWindow();
        }


        private Window.Window BuildMenuWindow()
        {
            ResizeOptionsToWidestWidth(options);

            IRenderable[,] optionWindows = new IRenderable[options.Length, 1];

            for (int i = 0; i < options.Length; i++)
            {
                optionWindows[i, 0] = options[i];
            }

            WindowContentGrid optionsContent = new WindowContentGrid(optionWindows, Padding);

            return new Window.Window("VerticalMenu", AssetManager.WindowTexture, optionsContent, backgroundColor);
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

        public int Height
        {
            get { return menuWindow.Height; }
        }


        public int Width
        {
            get { return menuWindow.Width; }
        }

        private Dictionary<MenuOption, Vector2> MapOptionCoordinates()
        {
            Dictionary<MenuOption, Vector2> optionCoordinatesMapping = new Dictionary<MenuOption, Vector2>();

            Vector2 lastCoordinates =
                new Vector2(AssetManager.WindowTexture.Width, AssetManager.WindowTexture.Height) / 3;

            for (int i = 0; i < options.Length; i++)
            {
                optionCoordinatesMapping[options[i]] = lastCoordinates;
                lastCoordinates.Y += options[i].Height + Padding;
            }

            return optionCoordinatesMapping;
        }


        public MenuOption CurrentOption
        {
            get { return options[CurrentOptionIndex]; }
        }

        public void SelectOption()
        {
            options[CurrentOptionIndex].Execute();
            AssetManager.MenuConfirmSFX.Play();
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
        {
            switch (direction)
            {
                case MenuCursorDirection.Forward:
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
                case MenuCursorDirection.Backward:
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
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        private void PositionCursorToOption()
        {
            MenuOption currentOption = options[CurrentOptionIndex];

            Vector2 optionPosition = optionCoordinates[currentOption];

            optionPosition.Y += (float) currentOption.Height / 2;

            cursorPosition = new Vector2(
                optionPosition.X - cursorSprite.Width,
                optionPosition.Y - (float) cursorSprite.Height / 2
            );
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, backgroundColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            menuWindow.Draw(spriteBatch, position, colorOverride);
            cursorSprite.Draw(spriteBatch, position + cursorPosition);
        }
    }
}
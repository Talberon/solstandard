using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu
{
    public class TwoDimensionalMenu : IRenderable
    {
        public enum MenuCursorDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        private readonly IRenderable cursorSprite;
        private Vector2 cursorPosition;

        private readonly MenuOption[,] options;

        private int CurrentOptionRow { get; set; }
        private int CurrentOptionColumn { get; set; }

        public Color DefaultColor { private get; set; }
        private const int Padding = 2;

        private readonly Window.Window menuWindow;
        private Vector2 optionSize;

        public bool IsVisible { get; set; }

        public TwoDimensionalMenu(MenuOption[,] options, IRenderable cursorSprite, Color color)
        {
            this.options = options;
            this.cursorSprite = cursorSprite;
            DefaultColor = color;
            IsVisible = true;

            menuWindow = BuildMenuWindow();
            SetCursorPosition(0, 0);
        }

        public int Height
        {
            get { return menuWindow.Height; }
        }

        public int Width
        {
            get { return menuWindow.Width; }
        }

        private void SetCursorPosition(int row, int column)
        {
            Vector2 optionPosition = new Vector2(column * optionSize.X, row * optionSize.Y);

            Vector2 centerLeft =
                new Vector2(cursorSprite.Width, ((float) cursorSprite.Height / 2) - (optionSize.Y / 2));

            cursorPosition = optionPosition - centerLeft;
        }

        private Window.Window BuildMenuWindow()
        {
            EqualizeOptionSizes(options);

            WindowContentGrid menuContent = new WindowContentGrid(options, Padding, HorizontalAlignment.Centered);

            return new Window.Window(menuContent, DefaultColor);
        }

        private void EqualizeOptionSizes(MenuOption[,] optionWindows)
        {
            List<MenuOption> flattened = optionWindows.Cast<MenuOption>().ToList();
            int maxHeight = flattened.Max(option => option.Height);
            int maxWidth = flattened.Max(option => option.Width);

            optionSize = new Vector2(maxWidth, maxHeight);

            foreach (MenuOption option in optionWindows)
            {
                option.Height = maxHeight;
                option.Width = maxWidth;
            }
        }

        public void SelectOption()
        {
            options[CurrentOptionRow, CurrentOptionColumn].Execute();
            AssetManager.MenuConfirmSFX.Play();
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
        {
            switch (direction)
            {
                case MenuCursorDirection.Up:
                    if (CurrentOptionRow > 0)
                    {
                        CurrentOptionRow--;
                    }
                    else
                    {
                        CurrentOptionRow = options.GetLength(0) - 1;
                    }

                    break;
                case MenuCursorDirection.Down:
                    if (CurrentOptionRow < options.GetLength(0) - 1)
                    {
                        CurrentOptionRow++;
                    }
                    else
                    {
                        CurrentOptionRow = 0;
                    }

                    break;
                case MenuCursorDirection.Left:
                    if (CurrentOptionColumn > 0)
                    {
                        CurrentOptionColumn--;
                    }
                    else
                    {
                        CurrentOptionColumn = options.GetLength(1) - 1;
                    }

                    break;
                case MenuCursorDirection.Right:
                    if (CurrentOptionColumn < options.GetLength(1) - 1)
                    {
                        CurrentOptionColumn++;
                    }
                    else
                    {
                        CurrentOptionColumn = 0;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            SetCursorPosition(CurrentOptionRow, CurrentOptionColumn);
            AssetManager.MenuMoveSFX.Play();
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            if (!IsVisible) return;
            menuWindow.Draw(spriteBatch, position, colorOverride);
            cursorSprite.Draw(spriteBatch, position + cursorPosition);
        }

        public IRenderable Clone()
        {
            return new TwoDimensionalMenu(options, cursorSprite, DefaultColor);
        }
    }
}
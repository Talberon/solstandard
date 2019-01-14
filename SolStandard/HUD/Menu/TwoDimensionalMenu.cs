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
        private readonly Dictionary<MenuOption, Vector2> optionCoordinates;

        private int CurrentOptionRow { get; set; }
        private int CurrentOptionColumn { get; set; }

        public Color DefaultColor { private get; set; }
        private const int Padding = 2;

        private readonly Window.Window menuWindow;

        public bool IsVisible { get; set; }

        public TwoDimensionalMenu(MenuOption[,] options, IRenderable cursorSprite, Color color)
        {
            this.options = options;
            this.cursorSprite = cursorSprite;
            DefaultColor = color;
            IsVisible = true;

            menuWindow = BuildMenuWindow();
        }

        public int Height
        {
            get { return menuWindow.Height; }
        }

        public int Width
        {
            get { return menuWindow.Width; }
        }

        private Vector2 SetCursorPosition(MenuOption option)
        {
            //TODO Set this to a menu option position
            return Vector2.One;
        }

        private Window.Window BuildMenuWindow()
        {
            EqualizeOptionSizes(options);

            WindowContentGrid menuContent = new WindowContentGrid(options, Padding, HorizontalAlignment.Centered);

            return new Window.Window(menuContent, DefaultColor);
        }

        private static void EqualizeOptionSizes(MenuOption[,] optionWindows)
        {
            List<MenuOption> flattened = optionWindows.Cast<MenuOption>().ToList();
            int maxHeight = flattened.Max(option => option.Height);
            int maxWidth = flattened.Max(option => option.Width);

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
                    //TODO Handle menu movement in 4 directions
                    break;
                case MenuCursorDirection.Down:
                    break;
                case MenuCursorDirection.Left:
                    break;
                case MenuCursorDirection.Right:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
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
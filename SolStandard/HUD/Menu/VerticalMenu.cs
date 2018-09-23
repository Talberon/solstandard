using System;
using System.Collections.Generic;
using System.Linq;
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
        private int currentOptionIndex;
        private readonly Color backgroundColor;
        private const int Padding = 2;
        private readonly Window.Window menuWindow;

        public bool Visible { get; set; }

        public VerticalMenu(MenuOption[] options, IRenderable cursorSprite, Color backgroundColor)
        {
            this.options = options;
            this.cursorSprite = cursorSprite;
            this.backgroundColor = backgroundColor;
            currentOptionIndex = 0;
            cursorPosition = Vector2.Zero;
            optionCoordinates = MapOptionCoordinates();
            PositionCursorToOption();
            menuWindow = BuildMenuWindow();
            Visible = true;
        }

        private Window.Window BuildMenuWindow()
        {
            IRenderable[,] optionWindows = new IRenderable[options.Length, 1];
            for (int i = 0; i < options.Length; i++)
            {
                optionWindows[i, 0] = options[i];
            }

            WindowContentGrid menuWindowContent = new WindowContentGrid(optionWindows, Padding);

            return new Window.Window("VerticalMenu", AssetManager.WindowTexture, menuWindowContent, backgroundColor);
        }

        public int Height
        {
            get { return (int) (optionCoordinates[options.Last()].Y + options.Last().Height); }
        }


        public int Width
        {
            //Get widest width of the available options
            get { return options.Select(option => option.Width).Concat(new[] {0}).Max(); }
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
            get { return options[currentOptionIndex]; }
        }

        public void SelectOption()
        {
            options[currentOptionIndex].Execute();
            AssetManager.MenuConfirmSFX.Play();
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
        {
            switch (direction)
            {
                case MenuCursorDirection.Forward:
                    if (currentOptionIndex < options.Length - 1)
                    {
                        currentOptionIndex++;
                        PositionCursorToOption();
                        AssetManager.MenuMoveSFX.Play();
                    }

                    break;
                case MenuCursorDirection.Backward:
                    if (currentOptionIndex > 0)
                    {
                        currentOptionIndex--;
                        PositionCursorToOption();
                        AssetManager.MenuMoveSFX.Play();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        private void PositionCursorToOption()
        {
            MenuOption currentOption = options[currentOptionIndex];

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
            if (!Visible) return;

            menuWindow.Draw(spriteBatch, position, colorOverride);
            cursorSprite.Draw(spriteBatch, position + cursorPosition);
        }
    }
}
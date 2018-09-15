using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu.Options;
using SolStandard.Utility;

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
        private readonly IOption[] options;
        private readonly Dictionary<IOption, Vector2> optionCoordinates;
        private int currentOptionIndex;
        private readonly Color optionBackgroundColor;
        private const int Padding = 2;


        public VerticalMenu(IOption[] options, IRenderable cursorSprite)
        {
            this.options = options;
            this.cursorSprite = cursorSprite;
            optionBackgroundColor = new Color(50, 100, 75, 200);
            currentOptionIndex = 0;
            cursorPosition = Vector2.Zero;
            optionCoordinates = MapOptionCoordinates();
            PositionCursorToOption();
        }

        public int Height
        {
            get { return (int) (optionCoordinates[options.Last()].Y + options.Last().OptionWindow.Height); }
        }


        public int Width
        {
            //Get widest width of the available options
            get { return options.Select(option => option.OptionWindow.Width).Concat(new[] {0}).Max(); }
        }

        private Dictionary<IOption, Vector2> MapOptionCoordinates()
        {
            Dictionary<IOption, Vector2> optionCoordinatesMapping = new Dictionary<IOption, Vector2>();

            Vector2 lastCoordinates = new Vector2(0);

            for (int i = 0; i < options.Length; i++)
            {
                optionCoordinatesMapping[options[i]] = lastCoordinates;
                lastCoordinates.Y += options[i].OptionWindow.Height + Padding;
            }

            return optionCoordinatesMapping;
        }


        public void SelectOption()
        {
            options[currentOptionIndex].Execute();
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
        {
            switch (direction)
            {
                case MenuCursorDirection.Forward:
                    if (currentOptionIndex < options.Length - 1) currentOptionIndex++;
                    PositionCursorToOption();
                    break;
                case MenuCursorDirection.Backward:
                    if (currentOptionIndex > 0) currentOptionIndex--;
                    PositionCursorToOption();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        private void PositionCursorToOption()
        {
            IOption currentOption = options[currentOptionIndex];

            Vector2 optionPosition = optionCoordinates[currentOption];

            optionPosition.Y += (float) currentOption.OptionWindow.Height / 2;

            cursorPosition = new Vector2(
                optionPosition.X - cursorSprite.Width,
                optionPosition.Y - (float) cursorSprite.Height / 2
            );
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            foreach (IOption option in options)
            {
                option.OptionWindow.Draw(spriteBatch, position + optionCoordinates[option], optionBackgroundColor);
            }

            cursorSprite.Draw(spriteBatch, position + cursorPosition);
        }
    }
}
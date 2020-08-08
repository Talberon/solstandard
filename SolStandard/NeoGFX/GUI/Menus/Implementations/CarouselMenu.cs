using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.NeoUtility.Directions;
using SolStandard.NeoUtility.General;

namespace SolStandard.NeoGFX.GUI.Menus.Implementations
{
    public class CarouselMenu : GridMenu
    {
        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        //Do not call options from here; they are only references for the hover effects.
        private readonly Orientation orientation;
        private readonly Vector2 centerPosition;
        private readonly int elementSpacing;

        public CarouselMenu(
            List<MenuOption> oneDimensionalOptions,
            Orientation orientation,
            Vector2 centerPosition,
            int elementSpacing = 0,
            Point? initialCursorPosition = null,
            HorizontalAlignment alignment = HorizontalAlignment.Centered
        ) : base(
            OptionsForOrientation(oneDimensionalOptions, orientation),
            centerPosition,
            elementSpacing,
            initialCursorPosition,
            alignment
        )
        {
            this.orientation = orientation;
            this.centerPosition = centerPosition;
            this.elementSpacing = elementSpacing;
            AdjustWindowToCurrentOption();
        }

        public override void MoveCursor(CardinalDirection direction)
        {
            base.MoveCursor(direction);

            AdjustWindowToCurrentOption();
        }

        private void AdjustWindowToCurrentOption()
        {
            (float centerX, float centerY) =
                CursorPosition.ToVector2().Inverted() * new Vector2(CurrentOption.Width, CurrentOption.Height);

            switch (orientation)
            {
                case Orientation.Horizontal:
                    //Slide horizontally
                    var nextHorizontalPosition =
                        new Vector2(
                            centerX - (CursorPosition.X * elementSpacing) + centerPosition.X - CurrentOption.Width / 2,
                            TopLeftPoint.Y
                        );
                    OptionsWindow.JuiceBox.MoveTowards(nextHorizontalPosition);
                    break;
                case Orientation.Vertical:
                    //Slide vertically
                    var nextVerticalPosition =
                        new Vector2(
                            TopLeftPoint.X,
                            centerY - (CursorPosition.Y * elementSpacing) + centerPosition.Y - CurrentOption.Height / 2
                        );
                    OptionsWindow.JuiceBox.MoveTowards(nextVerticalPosition);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static List<List<MenuOption>> OptionsForOrientation(List<MenuOption> options, Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Horizontal => new List<List<MenuOption>> {options},
                Orientation.Vertical => VerticalMenu.ConvertToVerticalList(options),
                _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
            };
        }

        public static MenuOption GenerateOption(MenuOption.OnConfirm onConfirm, string optionText,
            Color highlightedColor, Color inactiveColor)
        {
            return new AdHocMenuOption(
                onConfirm,
                (option) =>
                {
                    option.JuiceBox.FadeTowards(1);
                    option.JuiceBox.ShiftToNewSize(option.JuiceBox.DefaultSize);
                    option.JuiceBox.HueShiftTo(highlightedColor);
                },
                (option) =>
                {
                    option.JuiceBox.HueShiftTo(inactiveColor);
                    option.JuiceBox.ShiftToNewSize(option.JuiceBox.DefaultSize);
                },
                optionText,
                inactiveColor
            );
        }

        public static MenuOption GenerateOption(MenuOption.OnConfirm onConfirm, Window.JuicyWindow juicyWindow,
            Color highlightedColor, Color inactiveColor, MenuOption.OnHover? onHover = null)
        {
            return new AdHocMenuOption(
                onConfirm,
                (option) =>
                {
                    option.JuiceBox.FadeTowards(1);
                    option.JuiceBox.ShiftToNewSize(option.JuiceBox.DefaultSize);
                    option.JuiceBox.HueShiftTo(highlightedColor);
                    onHover?.Invoke(option);
                },
                (option) =>
                {
                    option.JuiceBox.HueShiftTo(inactiveColor);
                    option.JuiceBox.ShiftToNewSize(option.JuiceBox.DefaultSize);
                },
                juicyWindow
            );
        }
    }
}
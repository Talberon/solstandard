using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SolStandard.NeoGFX.Graphics;
using SolStandard.NeoGFX.Juice;
using SolStandard.NeoUtility.Directions;
using SolStandard.NeoUtility.General;
using SolStandard.NeoUtility.Monogame.Assets;

namespace SolStandard.NeoGFX.GUI.Menus
{
    public abstract class Menu : IPositionedRenderable
    {
        public static readonly Color DefaultOptionColor = new Color(50, 50, 50);

        protected abstract void OnOpen();

        public MenuOption CurrentOption => options[CursorPosition.Y][CursorPosition.X];

        protected Window.JuicyWindow OptionsWindow;
        protected Point CursorPosition { get; set; } //X = inner list, Y = outer list

        private List<List<MenuOption>> options; //Top-left to bottom-right
        private readonly Vector2 centerPosition;
        private readonly int elementSpacing;
        private readonly Point? initialCursorPosition;
        private readonly HorizontalAlignment alignment;

        // ReSharper disable once NotNullMemberIsNotInitialized
        protected Menu(List<List<MenuOption>> options, Vector2 centerPosition, int elementSpacing = 0,
            Point? initialCursorPosition = null, HorizontalAlignment alignment = HorizontalAlignment.Centered)
        {
            this.options = options;
            this.centerPosition = centerPosition;
            this.elementSpacing = elementSpacing;
            this.initialCursorPosition = initialCursorPosition;
            this.alignment = alignment;
            Reset();
        }

        protected void Reset(List<List<MenuOption>>? newOptions = null)
        {
            if (newOptions is object) options = newOptions;

            CursorPosition = initialCursorPosition ?? Point.Zero;

            {
                //Build window
                EqualizeOptionSizes(options.Flatten());

                var optionContainerWindowContentBuilder = new WindowContentGrid.Builder();

                foreach (List<MenuOption> row in options)
                {
                    var content = new List<IWindowContent>();
                    content.AddRange(row);
                    optionContainerWindowContentBuilder.AddRowOfContent(content.ToArray());
                }

                WindowContentGrid windowContentGrid = optionContainerWindowContentBuilder
                    .Spacing(elementSpacing)
                    .HorizontalAlignment(alignment)
                    .Build();

                OptionsWindow = new Window.Builder()
                    .Content(windowContentGrid)
                    .WindowColor(Color.Transparent)
                    .CenterOf(new RectangleF(centerPosition / 2, centerPosition))
                    .Texture(AssetManager.WhitePixel)
                    .HorizontalAlignment(alignment)
                    .Build()
                    .ToJuicyWindow();
            }

            CurrentOption.Hover(CurrentOption);
        }

        public void Open()
        {
            OnOpen();
        }

        /// <summary>
        /// Close the current window. May have side-effects.
        /// </summary>
        /// <returns>True if this menu can be closed. False if it cannot be cancelled.</returns>
        public virtual bool Close()
        {
            return true;
        }

        public virtual void MoveCursor(CardinalDirection direction)
        {
            var previousPosition = new Point(CursorPosition.X, CursorPosition.Y);
            MenuOption previousOption = CurrentOption;

            Point offset = direction switch
            {
                CardinalDirection.North => new Point(0, -1),
                CardinalDirection.South => new Point(0, 1),
                CardinalDirection.West => new Point(-1, 0),
                CardinalDirection.East => new Point(1, 0),
                _ => throw new ArgumentOutOfRangeException()
            };

            CursorPosition += offset;

            CursorPosition = CorrectCursorIfOutOfBounds();

            if (previousPosition == CursorPosition) return;

            SoundBox.MenuMove.Play();
            previousOption.Unhover(previousOption);
            CurrentOption.Hover(CurrentOption);
        }

        public void Confirm()
        {
            CurrentOption.Confirm(CurrentOption);
        }

        private Point CorrectCursorIfOutOfBounds()
        {
            (int cursorX, int cursorY) = CursorPosition;

            var nextPoint = new Point(cursorX, cursorY);

            if (cursorY < 0) nextPoint.Y = 0;
            if (cursorY >= options.Count) nextPoint.Y = options.Count - 1;
            if (cursorX < 0) nextPoint.X = 0;
            int rowItemCount = options[nextPoint.Y].Count;
            if (cursorX >= rowItemCount) nextPoint.X = rowItemCount - 1;

            return nextPoint;
        }

        private static void EqualizeOptionSizes(IList<MenuOption> menuOptions)
        {
            float maxHeight = menuOptions.Max(option => option.Height);
            float maxWidth = menuOptions.Max(option => option.Width);

            var optionSize = new Vector2(maxWidth, maxHeight);

            foreach (MenuOption option in menuOptions)
            {
                option.ImmediatelyOverrideSize(optionSize);
            }
        }

        //Window stuff

        public float Width => OptionsWindow.Width;
        public float Height => OptionsWindow.Height;

        public void Update(GameTime gameTime)
        {
            OptionsWindow.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            OptionsWindow.Draw(spriteBatch, OptionsWindow.JuiceBox.CurrentDrawPosition);
        }

        public Vector2 TopLeftPoint
        {
            get => OptionsWindow.JuiceBox.CurrentRealPosition;
            set => OptionsWindow.JuiceBox.SnapTo(value);
        }
    }
}
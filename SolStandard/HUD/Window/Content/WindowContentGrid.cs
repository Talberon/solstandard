using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content
{
    public class WindowContentGrid : IRenderable
    {
        public Color DefaultColor { get; set; }
        private readonly List<List<IRenderable>> contentGrid;
        private readonly int padding;

        private HorizontalAlignment HorizontalAlignment { get; set; }

        private WindowContentGrid(List<List<IRenderable>> contentGrid, int padding, HorizontalAlignment alignment)
        {
            this.contentGrid = contentGrid;
            this.padding = padding;
            HorizontalAlignment = alignment;
            DefaultColor = Color.White;
        }

        public WindowContentGrid(IRenderable[,] contentGrid, int padding,
            HorizontalAlignment alignment = HorizontalAlignment.Left)
            : this(ArrayToList<IRenderable>.Convert2DArrayToNestedList(contentGrid), padding, alignment)
        {
        }

        public int Height
        {
            get { return (int) GridSizeInPixels().Y; }
        }

        public int Width
        {
            get { return (int) GridSizeInPixels().X; }
        }

        public Vector2 GridSizeInPixels()
        {
            float totalWidth = 0f;
            float totalHeight = 0;

            foreach (List<IRenderable> row in contentGrid)
            {
                int rowWidth = row.Sum(item => item.Width);
                if (rowWidth > totalWidth) totalWidth = rowWidth + row.Count * padding;
                totalHeight += row.Max(item => item.Height) + padding;
            }

            return new Vector2(totalWidth, totalHeight);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            Draw(spriteBatch, coordinates, DefaultColor);
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates, Color colorOverride)
        {
            float previousHeight = 0f;

            foreach (List<IRenderable> row in contentGrid)
            {
                float rowWidth = row.Sum(item => item.Width);

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        DrawRow(spriteBatch, row, new Vector2(coordinates.X, coordinates.Y + previousHeight));
                        break;
                    case HorizontalAlignment.Centered:
                        DrawRow(
                            spriteBatch,
                            row,
                            new Vector2(
                                coordinates.X + ((float) Width / 2 - rowWidth / 2),
                                coordinates.Y + previousHeight
                            )
                        );
                        break;
                    case HorizontalAlignment.Right:
                        DrawRow(
                            spriteBatch,
                            row,
                            new Vector2(
                                coordinates.X + (Width - rowWidth),
                                coordinates.Y + previousHeight
                            )
                        );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                previousHeight += row.Max(item => item.Height) + padding;
            }
        }

        public IRenderable Clone()
        {
            return new WindowContentGrid(contentGrid, padding, HorizontalAlignment);
        }

        private void DrawRow(SpriteBatch spriteBatch, IEnumerable<IRenderable> row, Vector2 coordinates)
        {
            float horizontalOffset = 0f;
            foreach (IRenderable item in row)
            {
                item.Draw(spriteBatch, new Vector2(coordinates.X + horizontalOffset, coordinates.Y));
                horizontalOffset += item.Width + padding;
            }
        }
    }
}
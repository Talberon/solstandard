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
        private readonly int spacing;
        private readonly List<List<IRenderable>> gridContents;
        private HorizontalAlignment HorizontalAlignment { get; }

        private WindowContentGrid(List<List<IRenderable>> contentGrid, int spacing, HorizontalAlignment alignment)
        {
            gridContents = contentGrid;
            this.spacing = spacing;
            HorizontalAlignment = alignment;
            DefaultColor = Color.Transparent;
        }

        public WindowContentGrid(IRenderable[,] contentGrid, int spacing = 1,
            HorizontalAlignment alignment = HorizontalAlignment.Left)
            : this(ArrayToList<IRenderable>.Convert2DArrayToNestedList(contentGrid), spacing, alignment)
        {
        }


        public int Height => (int) GridSizeInPixels().Y;
        public int Width => (int) GridSizeInPixels().X;

        private Vector2 GridSizeInPixels()
        {
            float totalWidth = 0f;
            float totalHeight = 0;

            foreach (List<IRenderable> row in gridContents)
            {
                int rowWidth = row.Sum(item => item.Width) + row.Count * spacing;
                if (rowWidth > totalWidth) totalWidth = rowWidth;
                totalHeight += row.Max(item => item.Height) + spacing;
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

            foreach (List<IRenderable> row in gridContents)
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

                previousHeight += row.Max(item => item.Height) + spacing;
            }
        }

        public IRenderable Clone()
        {
            return new WindowContentGrid(gridContents, spacing, HorizontalAlignment);
        }

        private void DrawRow(SpriteBatch spriteBatch, IEnumerable<IRenderable> row, Vector2 coordinates)
        {
            float horizontalOffset = 0f;
            foreach (IRenderable item in row)
            {
                item.Draw(spriteBatch, new Vector2(coordinates.X + horizontalOffset, coordinates.Y));
                horizontalOffset += item.Width + spacing;
            }
        }
    }
}
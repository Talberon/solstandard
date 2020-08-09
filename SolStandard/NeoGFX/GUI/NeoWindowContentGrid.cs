using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SolStandard.NeoUtility.General;

namespace SolStandard.NeoGFX.GUI
{
    public class NeoWindowContentGrid : IWindowContent
    {
        private readonly Vector2 position;
        private int spacing;
        private readonly List<List<IWindowContent>> gridContents; //Column<Row<Content>>
        private HorizontalAlignment horizontalAlignment;
        public float Height => GridSizeInPixels.Y;
        public float Width => GridSizeInPixels.X;

        private NeoWindowContentGrid(List<List<IWindowContent>> contentGrid, int spacing, HorizontalAlignment alignment,
            Vector2 position)
        {
            gridContents = contentGrid;
            this.spacing = spacing;
            horizontalAlignment = alignment;
            this.position = position;
        }

        public NeoWindowContentGrid(IWindowContent[,] contentGrid, Vector2? position = null, int spacing = 1,
            HorizontalAlignment alignment = HorizontalAlignment.Left)
            : this(ArrayToList<IWindowContent>.Convert2DArrayToNestedList(contentGrid), spacing, alignment,
                position ?? Vector2.Zero)
        {
        }

        private Vector2 GridSizeInPixels => DimensionsForTwoDimensionalList(gridContents, spacing);

        public static Vector2 DimensionsForTwoDimensionalList(List<List<IWindowContent>> gridContents, float spacing)
        {
            float totalWidth = 0f;
            float totalHeight = 0;

            foreach (List<IWindowContent> row in gridContents)
            {
                float rowWidth = row.Sum(item => item.Width) + row.Count * spacing;
                if (rowWidth > totalWidth) totalWidth = rowWidth;
                totalHeight += row.Max(item => item.Height) + spacing;
            }

            return new Vector2(totalWidth, totalHeight);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            float previousHeight = 0f;

            foreach (List<IWindowContent> row in gridContents)
            {
                float rowWidth = row.Sum(item => item.Width + ((spacing > 0) ? spacing : 0));

                (float drawX, float drawY) = coordinates;

                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        DrawRow(spriteBatch, row, new Vector2(drawX, drawY + previousHeight));
                        break;
                    case HorizontalAlignment.Centered:
                        DrawRow(
                            spriteBatch,
                            row,
                            new Vector2(
                                drawX + (Width / 2 - rowWidth / 2),
                                drawY + previousHeight
                            )
                        );
                        break;
                    case HorizontalAlignment.Right:
                        DrawRow(
                            spriteBatch,
                            row,
                            new Vector2(
                                drawX + (Width - rowWidth),
                                drawY + previousHeight
                            )
                        );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                previousHeight += row.Max(item => item.Height) + spacing;
            }

            if (GameDriver.DebugMode)
            {
                spriteBatch.DrawRectangle(new RectangleF(coordinates.X, coordinates.Y, Width, Height), Color.Pink);
            }
        }

        public void Update(GameTime gameTime)
        {
            gridContents.ForEach(list => list.ForEach(item => item.Update(gameTime)));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, position);
        }

        private void DrawRow(SpriteBatch spriteBatch, IEnumerable<IWindowContent> row, Vector2 coordinates)
        {
            float horizontalOffset = 0f;
            foreach (IWindowContent item in row)
            {
                item.Draw(spriteBatch, new Vector2(coordinates.X + horizontalOffset, coordinates.Y));
                horizontalOffset += item.Width + spacing;
            }
        }

        public class Builder
        {
            private readonly NeoWindowContentGrid grid;

            public Builder()
            {
                grid = new NeoWindowContentGrid(new IWindowContent[,] {{ }});
            }

            private Builder(NeoWindowContentGrid grid)
            {
                this.grid = grid;
            }

            public static Builder Mutate(NeoWindowContentGrid grid)
            {
                return new Builder(grid);
            }

            public Builder SetContent(IWindowContent content)
            {
                grid.gridContents.Clear();
                grid.gridContents.Add(new List<IWindowContent> {content});
                return this;
            }

            public Builder AddContentToRow(IWindowContent content, int rowIndex)
            {
                if (rowIndex >= grid.gridContents.Count)
                {
                    throw new IndexOutOfRangeException(
                        $"Tried to access row {{row}}, but only had {grid.gridContents.Count} rows available."
                    );
                }

                grid.gridContents[rowIndex].Add(content);
                return this;
            }

            public Builder AddContentToRow(IWindowContent content)
            {
                return AddContentToRow(content, grid.gridContents.Count - 1);
            }

            public Builder AddRowOfContent(params IWindowContent[] rowContents)
            {
                if (grid.gridContents[0].Count == 0) grid.gridContents.Clear();

                grid.gridContents.Add(rowContents.ToList());
                return this;
            }

            public Builder AddRowOfContent(IEnumerable<IWindowContent> rowContents)
            {
                if (grid.gridContents[0].Count == 0) grid.gridContents.Clear();

                grid.gridContents.Add(rowContents.ToList());
                return this;
            }

            public Builder AddBlankRow()
            {
                grid.gridContents.Add(new List<IWindowContent>());
                return this;
            }

            public Builder Spacing(int spacingPx)
            {
                grid.spacing = spacingPx;
                return this;
            }

            public Builder HorizontalAlignment(HorizontalAlignment alignment)
            {
                grid.horizontalAlignment = alignment;
                return this;
            }

            public NeoWindowContentGrid Build()
            {
                return grid;
            }
        }
    }
}
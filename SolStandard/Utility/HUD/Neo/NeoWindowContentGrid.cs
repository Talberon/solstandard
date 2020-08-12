using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace SolStandard.Utility.HUD.Neo
{
    public class NeoWindowContentGrid : IRenderable
    {
        private readonly Vector2 topLeftPoint;
        private int spacing;
        private readonly List<List<IRenderable>> gridContents; //Column<Row<Content>>
        private HorizontalAlignment horizontalAlignment;

        public float Height => GridSizeInPixels.Y;
        public float Width => GridSizeInPixels.X;

        int IRenderable.Width => (int) Width;
        int IRenderable.Height => (int) Height;

        public Color DefaultColor { get; set; } // Do not use

        private NeoWindowContentGrid(List<List<IRenderable>> contentGrid, int spacing, HorizontalAlignment alignment,
            Vector2 topLeftPoint)
        {
            gridContents = contentGrid;
            this.spacing = spacing;
            horizontalAlignment = alignment;
            this.topLeftPoint = topLeftPoint;
        }

        public NeoWindowContentGrid(IRenderable[,] contentGrid, Vector2? position = null, int spacing = 1,
            HorizontalAlignment alignment = HorizontalAlignment.Left)
            : this(ArrayToList<IRenderable>.Convert2DArrayToNestedList(contentGrid), spacing, alignment,
                position ?? Vector2.Zero)
        {
        }

        private Vector2 GridSizeInPixels => DimensionsForTwoDimensionalList(gridContents, spacing);

        public static Vector2 DimensionsForTwoDimensionalList(List<List<IRenderable>> gridContents, float spacing)
        {
            float totalWidth = 0f;
            float totalHeight = 0;

            foreach (List<IRenderable> row in gridContents)
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

            foreach (List<IRenderable> row in gridContents)
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

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color _)
        {
            Draw(spriteBatch, position);
        }

        public IRenderable Clone()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            // gridContents.ForEach(list => list.ForEach(item => item.Update(gameTime)));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, topLeftPoint);
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

        public class Builder
        {
            private readonly NeoWindowContentGrid grid;

            public Builder()
            {
                grid = new NeoWindowContentGrid(new IRenderable[,] {{ }});
            }

            private Builder(NeoWindowContentGrid grid)
            {
                this.grid = grid;
            }

            public static Builder Mutate(NeoWindowContentGrid grid)
            {
                return new Builder(grid);
            }

            public Builder SetContent(IRenderable content)
            {
                grid.gridContents.Clear();
                grid.gridContents.Add(new List<IRenderable> {content});
                return this;
            }

            public Builder AddContentToRow(IRenderable content, int rowIndex)
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

            public Builder AddContentToRow(IRenderable content)
            {
                return AddContentToRow(content, grid.gridContents.Count - 1);
            }

            public Builder AddRowOfContent(params IRenderable[] rowContents)
            {
                if (grid.gridContents[0].Count == 0) grid.gridContents.Clear();

                grid.gridContents.Add(rowContents.ToList());
                return this;
            }

            public Builder AddRowOfContent(IEnumerable<IRenderable> rowContents)
            {
                if (grid.gridContents[0].Count == 0) grid.gridContents.Clear();

                grid.gridContents.Add(rowContents.ToList());
                return this;
            }

            public Builder AddBlankRow()
            {
                grid.gridContents.Add(new List<IRenderable>());
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
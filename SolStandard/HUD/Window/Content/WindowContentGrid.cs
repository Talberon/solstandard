using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content
{
    public class WindowContentGrid : IRenderable
    {
        private readonly IRenderable[,] contentGrid;
        private readonly int padding;

        public WindowContentGrid(IRenderable[,] contentGrid, int padding)
        {
            this.contentGrid = contentGrid;
            this.padding = padding;
        }

        private IRenderable[,] ContentGrid
        {
            get { return contentGrid; }
        }

        public int Height
        {
            get { return (int) GridSizeInPixels().Y; }
        }

        public int Width
        {
            get { return (int) GridSizeInPixels().X; }
        }

        //TODO clean this so I'm not duplicating so much of the Draw logic (maybe use Delegates)
        public Vector2 GridSizeInPixels()
        {
            float totalWidth = 0f;
            float totalHeight = 0;

            float highestRowHeight = 0f;
            float horizontalOffset = 0f;

            for (int column = 0; column < ContentGrid.GetLength(0); column++)
            {
                for (int row = 0; row < ContentGrid.GetLength(1); row++)
                {
                    float contentWidth = ContentGrid[column, row].Width;
                    horizontalOffset += contentWidth + padding;

                    float contentHeight = ContentGrid[column, row].Height;
                    if (highestRowHeight < contentHeight)
                    {
                        highestRowHeight = contentHeight + padding;
                    }
                }

                //Combination of highest heights determines the height
                totalHeight += highestRowHeight;

                //Widest set of items determines the width
                if (totalWidth < horizontalOffset)
                {
                    totalWidth = horizontalOffset;
                }

                horizontalOffset = 0;
                highestRowHeight = 0;
            }

            return new Vector2(totalWidth, totalHeight);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            Draw(spriteBatch, coordinates, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates, Color colorOverride)
        {
            float highestRowHeight = 0f;

            float horizontalOffset = 0f;
            float verticalOffset = 0f;

            for (int column = 0; column < ContentGrid.GetLength(0); column++)
            {
                for (int row = 0; row < ContentGrid.GetLength(1); row++)
                {
                    //Draw with offset
                    ContentGrid[column, row].Draw(spriteBatch,
                        new Vector2(coordinates.X + horizontalOffset, coordinates.Y + verticalOffset));

                    //Adjust offset
                    float contentWidth = ContentGrid[column, row].Width;
                    horizontalOffset += contentWidth + padding;

                    float contentHeight = ContentGrid[column, row].Height;
                    if (highestRowHeight < contentHeight)
                    {
                        highestRowHeight = contentHeight + padding;
                    }
                }

                verticalOffset += highestRowHeight; //Once I start drawing the next row I should reset the height
                highestRowHeight = 0;
                horizontalOffset = 0;
            }
        }
    }
}
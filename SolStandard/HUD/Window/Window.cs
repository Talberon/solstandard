using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window
{
    public class Window
    {
        private readonly ITexture2D windowTexture;
        private readonly int windowCellSize;
        private readonly WindowContentGrid windowContents;
        private readonly Vector2 windowPixelSize;
        private readonly WindowCell[,] windowCells;

        private readonly Vector2 windowPosition;
        private readonly int padding;

        //Single Content
        public Window(ITexture2D windowTexture, IRenderable windowContent, Vector2 windowPosition, int padding)
        {
            this.windowTexture = windowTexture;
            this.windowPosition = windowPosition;
            this.padding = padding;
            windowContents = new WindowContentGrid(new IRenderable[,] {{windowContent}});
            windowCellSize = CalculateCellSize(windowTexture);
            windowPixelSize = DeriveSizeFromContent(windowContents.ContentGrid);
            windowCells = ConstructWindowCells(WindowPixelSize);
        }

        //Grid of Content
        public Window(ITexture2D windowTexture, WindowContentGrid windowContents, Vector2 windowPosition, int padding)
        {
            this.windowTexture = windowTexture;
            this.windowContents = windowContents;
            this.windowPosition = windowPosition;
            this.padding = padding;
            windowCellSize = CalculateCellSize(windowTexture);
            windowPixelSize = DeriveSizeFromContent(this.windowContents.ContentGrid);
            windowCells = ConstructWindowCells(WindowPixelSize);
        }

        public Vector2 WindowPixelSize
        {
            get { return windowPixelSize; }
        }

        public Vector2 WindowPosition
        {
            get { return windowPosition; }
        }

        private int CalculateCellSize(ITexture2D windowTextureTemplate)
        {
            //Window Texture must be a square
            if (windowTextureTemplate.GetWidth() == windowTextureTemplate.GetHeight())
            {
                return windowTexture.GetHeight() / 3;
            }

            throw new InvalidWindowTextureException();
        }

        private WindowCell[,] ConstructWindowCells(Vector2 size)
        {
            WindowCell[,] windowCellsToConstruct = new WindowCell[(int) size.X, (int) size.Y];

            //Build the GameTile list
            for (int row = 0; row < (int) size.Y; row++)
            {
                for (int column = 0; column < (int) size.X; column++)
                {
                    //Top Border
                    int cellIndex;
                    if (row == 0)
                    {
                        //Top-Left Corner
                        if (column == 0)
                        {
                            cellIndex = 1;
                        }
                        //Top-Right Corner
                        else if (column == (int) size.X - 1)
                        {
                            cellIndex = 3;
                        }
                        //Top Border
                        else
                        {
                            cellIndex = 2;
                        }
                    }
                    //Bottom Border
                    else if (row == (int) size.Y - 1)
                    {
                        //Bottom-Left Corner
                        if (column == 0)
                        {
                            cellIndex = 7;
                        }
                        //Bottom-Right Corner
                        else if (column == (int) size.X - 1)
                        {
                            cellIndex = 9;
                        }
                        //Bottom Border
                        else
                        {
                            cellIndex = 8;
                        }
                    }
                    //Left Border
                    else if (column == 0)
                    {
                        cellIndex = 4;
                    }
                    //Right Border
                    else if (column == (int) size.X - 1)
                    {
                        cellIndex = 6;
                    }
                    //Background
                    else
                    {
                        cellIndex = 5;
                    }

                    TileCell windowTile = new TileCell(windowTexture, windowCellSize, cellIndex);

                    windowCellsToConstruct[column, row] = new WindowCell(windowTile, new Vector2(column, row));
                }
            }

            return windowCellsToConstruct;
        }

        private Vector2 DeriveSizeFromContent(IRenderable[,] contentGrid)
        {
            Vector2 calculatedSize = new Vector2();

            Vector2 contentGridSize = DetermineGridSizeInPixels();
            calculatedSize.X = contentGridSize.X;
            calculatedSize.Y = contentGridSize.Y;

            //Adjust for border
            int borderSize = windowCellSize * 2;
            calculatedSize.X += borderSize;
            calculatedSize.Y += borderSize;

            return calculatedSize;
        }


        private void RenderGrid(SpriteBatch spriteBatch)
        {
            Vector2 borderOffset = new Vector2(windowCellSize);
            Vector2 renderPosition = WindowPosition + borderOffset;

            float highestRowHeight = 0f;

            float horizontalOffset = 0f;
            float verticalOffset = 0f;

            for (int column = 0; column < windowContents.ContentGrid.GetLength(0); column++)
            {
                for (int row = 0; row < windowContents.ContentGrid.GetLength(1); row++)
                {
                    //Draw with offset
                    windowContents.ContentGrid[column, row].Draw(spriteBatch,
                        new Vector2(renderPosition.X + horizontalOffset, renderPosition.Y + verticalOffset));

                    //Adjust offset
                    float contentWidth = windowContents.ContentGrid[column, row].GetWidth();
                    horizontalOffset += contentWidth + padding;

                    float contentHeight = windowContents.ContentGrid[column, row].GetHeight();
                    if (highestRowHeight < contentHeight)
                    {
                        highestRowHeight = contentHeight + padding;
                    }
                }

                verticalOffset = highestRowHeight; //Once I start drawing the next row I should reset the height
                highestRowHeight = 0;
                horizontalOffset = 0;
            }
        }

        //TODO clean this so I'm not duplicating so much of the RenderGrid logic
        private Vector2 DetermineGridSizeInPixels()
        {
            float totalWidth = 0f;
            float totalHeight = 0;

            float highestRowHeight = 0f;
            float horizontalOffset = 0f;

            for (int column = 0; column < windowContents.ContentGrid.GetLength(0); column++)
            {
                for (int row = 0; row < windowContents.ContentGrid.GetLength(1); row++)
                {
                    float contentWidth = windowContents.ContentGrid[column, row].GetWidth();
                    horizontalOffset += contentWidth + padding;

                    float contentHeight = windowContents.ContentGrid[column, row].GetHeight();
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

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (WindowCell windowCell in windowCells)
            {
                windowCell.Draw(spriteBatch, WindowPosition);
            }

            RenderGrid(spriteBatch);
        }
    }
}
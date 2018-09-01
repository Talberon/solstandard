using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window
{
    public class Window : IRenderable
    {
        private readonly string windowLabel;
        private ITexture2D windowTexture;
        private readonly int windowCellSize;
        private readonly WindowContentGrid windowContents;
        private readonly Vector2 windowPixelSize;
        private readonly WindowCell[,] windowCells;
        private readonly Color windowColor;

        public bool Visible { get; set; }

        //Single Content
        public Window(string windowLabel, ITexture2D windowTexture, IRenderable windowContent, Color windowColor)
        {
            this.windowTexture = windowTexture;
            this.windowColor = windowColor;
            this.windowLabel = windowLabel;
            windowContents = new WindowContentGrid(new[,] {{windowContent}}, 0);
            windowCellSize = CalculateCellSize(windowTexture);
            windowPixelSize = DeriveSizeFromContent();
            windowCells = ConstructWindowCells(WindowPixelSize);
            Visible = true;
        }

        //Grid of Content
        public Window(string windowLabel, ITexture2D windowTexture, WindowContentGrid windowContents, Color windowColor)
        {
            this.windowTexture = windowTexture;
            this.windowContents = windowContents;
            this.windowColor = windowColor;
            this.windowLabel = windowLabel;
            windowCellSize = CalculateCellSize(windowTexture);
            windowPixelSize = DeriveSizeFromContent();
            windowCells = ConstructWindowCells(WindowPixelSize);
            Visible = true;
        }

        //Single Content TODO Figure out a shorter way to express this
        public Window(string windowLabel, ITexture2D windowTexture, IRenderable windowContent, Color windowColor,
            Vector2 windowPixelSize)
        {
            this.windowTexture = windowTexture;
            this.windowColor = windowColor;
            this.windowLabel = windowLabel;
            windowContents = new WindowContentGrid(new[,] {{windowContent}}, 0);
            windowCellSize = CalculateCellSize(windowTexture);
            this.windowPixelSize = DeriveSizeFromContent(windowPixelSize);
            windowCells = ConstructWindowCells(WindowPixelSize);
            Visible = true;
        }

        //Grid of Content TODO Figure out a shorter way to express this
        public Window(string windowLabel, ITexture2D windowTexture, WindowContentGrid windowContents, Color windowColor,
            Vector2 windowPixelSize)
        {
            this.windowTexture = windowTexture;
            this.windowContents = windowContents;
            this.windowColor = windowColor;
            this.windowLabel = windowLabel;
            windowCellSize = CalculateCellSize(windowTexture);
            this.windowPixelSize = DeriveSizeFromContent(windowPixelSize);
            windowCells = ConstructWindowCells(WindowPixelSize);
            Visible = true;
        }

        public Vector2 WindowPixelSize
        {
            get { return windowPixelSize; }
        }

        public string WindowLabel
        {
            get { return windowLabel; }
        }


        private int CalculateCellSize(ITexture2D windowTextureTemplate)
        {
            //Window Texture must be a square
            if (windowTextureTemplate.Width == windowTextureTemplate.Height)
            {
                return windowTexture.Height / 3;
            }

            throw new InvalidWindowTextureException();
        }

        private Vector2 DeriveSizeFromContent()
        {
            Vector2 calculatedSize = new Vector2();

            Vector2 contentGridSize = windowContents.GridSizeInPixels();
            calculatedSize.X = contentGridSize.X;
            calculatedSize.Y = contentGridSize.Y;

            //Adjust for border
            int borderSize = windowCellSize * 2;
            calculatedSize.X += borderSize;
            calculatedSize.Y += borderSize;

            return calculatedSize;
        }

        private Vector2 DeriveSizeFromContent(Vector2 sizeOverride)
        {
            Vector2 calculatedSize = new Vector2();
            Vector2 contentGridSize = windowContents.GridSizeInPixels();

            //Adjust for border
            int borderSize = windowCellSize * 2;

            //Default to content size if size is set to 0
            if (Math.Abs(sizeOverride.X) < .001)
            {
                calculatedSize.X = contentGridSize.X;
                calculatedSize.X += borderSize;
            }
            else
            {
                calculatedSize.X = sizeOverride.X;
            }

            if (Math.Abs(sizeOverride.Y) < .001)
            {
                calculatedSize.Y = contentGridSize.Y;
                calculatedSize.Y += borderSize;
            }
            else
            {
                calculatedSize.Y = sizeOverride.Y;
            }

            return calculatedSize;
        }

        /*
         * Window Cells
         * [1][2][3]
         * [4][5][6]
         * [7][8][9]
         */
        private WindowCell[,] ConstructWindowCells(Vector2 pixelSize)
        {
            WindowCell[,] windowCellsToConstruct =
                new WindowCell[(int) pixelSize.X / windowCellSize, (int) pixelSize.Y / windowCellSize];

            //Build the GameTile list
            for (int column = 0; column < windowCellsToConstruct.GetLength(0); column++)
            {
                for (int row = 0; row < windowCellsToConstruct.GetLength(1); row++)
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
                        else if (column == windowCellsToConstruct.GetLength(0) - 1)
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
                    else if (row == windowCellsToConstruct.GetLength(1) - 1)
                    {
                        //Bottom-Left Corner
                        if (column == 0)
                        {
                            cellIndex = 7;
                        }
                        //Bottom-Right Corner
                        else if (column == windowCellsToConstruct.GetLength(0) - 1)
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
                    else if (column == windowCellsToConstruct.GetLength(0) - 1)
                    {
                        cellIndex = 6;
                    }
                    //Background
                    else
                    {
                        cellIndex = 5;
                    }

                    windowCellsToConstruct[column, row] = new WindowCell(windowCellSize, cellIndex,
                        new Vector2(column * windowCellSize, row * windowCellSize));
                }
            }

            return windowCellsToConstruct;
        }

        public int Height
        {
            get { return (int) windowPixelSize.Y; }
        }

        public int Width
        {
            get { return (int) windowPixelSize.X; }
        }

        private Vector2 CenteredContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;

            contentRenderCoordinates.X += ((float) Width / 2) - (windowContents.GridSizeInPixels().X / 2);
            contentRenderCoordinates.Y += ((float) Height / 2) - (windowContents.GridSizeInPixels().Y / 2);

            contentRenderCoordinates.X = (float) Math.Round(contentRenderCoordinates.X);
            contentRenderCoordinates.Y = (float) Math.Round(contentRenderCoordinates.Y);

            return contentRenderCoordinates;
        }


        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            if (Visible)
            {
                foreach (WindowCell windowCell in windowCells)
                {
                    windowCell.Draw(spriteBatch, ref windowTexture, coordinates, windowColor);
                }

                windowContents.Draw(spriteBatch, CenteredContentCoordinates(coordinates));
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates, Color color)
        {
            if (Visible)
            {
                foreach (WindowCell windowCell in windowCells)
                {
                    windowCell.Draw(spriteBatch, ref windowTexture, coordinates, color);
                }

                windowContents.Draw(spriteBatch, CenteredContentCoordinates(coordinates));
            }
        }
    }
}
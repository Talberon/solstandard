using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window
{
    public enum HorizontalAlignment
    {
        Left,
        Centered,
        Right
    }

    public class Window : IRenderable
    {
        private readonly string windowName;
        private ITexture2D windowTexture;
        private readonly int windowCellSize;
        private readonly WindowContentGrid windowContents;
        private WindowCell[,] windowCells;
        private Color windowColor;
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public Vector2 WindowPixelSize { get; private set; }
        public bool Visible { get; set; }


        public Window(string windowName, ITexture2D windowTexture, IRenderable windowContent, Color windowColor) :
            this(windowName, windowTexture, windowContent, windowColor, Vector2.Zero)
        {
        }


        public Window(string windowName, ITexture2D windowTexture, IRenderable windowContent, Color windowColor,
            HorizontalAlignment horizontalAlignment) :
            this(windowName, windowTexture, windowContent, windowColor, Vector2.Zero, horizontalAlignment)
        {
        }

        public Window(string windowName, ITexture2D windowTexture, IRenderable windowContent, Color windowColor,
            Vector2 pixelSizeOverride, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Centered)
        {
            this.windowTexture = windowTexture;
            this.windowColor = windowColor;
            this.windowName = windowName;
            windowContents = new WindowContentGrid(new[,] {{windowContent}}, 0);
            windowCellSize = CalculateCellSize(windowTexture);
            WindowPixelSize = DeriveSizeFromContent(pixelSizeOverride);
            windowCells = ConstructWindowCells(WindowPixelSize);
            Visible = true;
            HorizontalAlignment = horizontalAlignment;
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
            get { return (int) WindowPixelSize.Y; }
            set
            {
                WindowPixelSize = DeriveSizeFromContent(new Vector2(0, value));
                windowCells = ConstructWindowCells(WindowPixelSize);
            }
        }

        public int Width
        {
            get { return (int) WindowPixelSize.X; }
            set
            {
                WindowPixelSize = DeriveSizeFromContent(new Vector2(value, 0));
                windowCells = ConstructWindowCells(WindowPixelSize);
            }
        }


        private Vector2 GetCoordinatesBasedOnAlignment(Vector2 windowCoordinates)
        {
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    return LeftAlignedContentCoordinates(windowCoordinates);
                    break;
                case HorizontalAlignment.Centered:
                    return CenteredContentCoordinates(windowCoordinates);
                    break;
                case HorizontalAlignment.Right:
                    RightAlignedContentCoordinates(windowCoordinates);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return windowCoordinates;
        }

        private Vector2 LeftAlignedContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;
            contentRenderCoordinates.X += windowCellSize;
            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private Vector2 CenteredContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;

            contentRenderCoordinates.X += ((float) Width / 2) - (windowContents.GridSizeInPixels().X / 2);
            contentRenderCoordinates.X = (float) Math.Round(contentRenderCoordinates.X);

            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private Vector2 RightAlignedContentCoordinates(Vector2 windowCoordinates)
        {
            Vector2 contentRenderCoordinates = windowCoordinates;

            contentRenderCoordinates.X = Width - windowContents.Width - windowCellSize;

            contentRenderCoordinates.Y = VerticalCenterContent(windowCoordinates);

            return contentRenderCoordinates;
        }

        private float VerticalCenterContent(Vector2 windowCoordinates)
        {
            float contentRenderCoordinates = windowCoordinates.Y;
            contentRenderCoordinates += ((float) Height / 2) - (windowContents.GridSizeInPixels().Y / 2);
            return (float) Math.Round(contentRenderCoordinates);
        }


        public int Alpha
        {
            set { windowColor.A = Convert.ToByte(value); }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            Draw(spriteBatch, coordinates, windowColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates, Color colorOverride)
        {
            if (Visible)
            {
                foreach (WindowCell windowCell in windowCells)
                {
                    windowCell.Draw(spriteBatch, ref windowTexture, coordinates, colorOverride);
                }

                windowContents.Draw(spriteBatch, GetCoordinatesBasedOnAlignment(coordinates));
            }
        }
    }
}
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
        private readonly List<IRenderable> windowContents;
        private readonly Vector2 windowPixelSize;
        private readonly WindowCell[,] windowCells;

        private readonly Vector2 windowPosition;
        //TODO add padding?

        //Derived Size
        public Window(ITexture2D windowTexture, List<IRenderable> windowContents, Vector2 windowPosition)
        {
            this.windowTexture = windowTexture;
            this.windowContents = windowContents;
            this.windowPosition = windowPosition;
            windowCellSize = CalculateCellSize(windowTexture);
            windowPixelSize = DeriveSizeFromContent(windowContents);
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

        private Vector2 DeriveSizeFromContent(IEnumerable<IRenderable> contents)
        {
            Vector2 calculatedSize = new Vector2();

            int borderSize = windowCellSize * 2;
            calculatedSize.X += borderSize;
            calculatedSize.Y += borderSize;
            
            foreach (IRenderable content in contents)
            {
                //TODO adjust this depending on content layout (possibly a table/grid layout)
                calculatedSize.X += content.GetWidth();
                calculatedSize.Y += content.GetHeight();
            }
            
            return calculatedSize;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (WindowCell windowCell in windowCells)
            {
                windowCell.Draw(spriteBatch, WindowPosition);
            }

            Vector2 borderOffset = new Vector2(windowCellSize);
            
            foreach (IRenderable content in windowContents)
            {
                content.Draw(spriteBatch, WindowPosition + borderOffset);
            }
        }
    }
}
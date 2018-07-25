using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SolStandard.Utility
{
    class TileCell : IRenderable
    {
        private Texture2D image;
        private int cellSize;
        private int cellIndex;

        public TileCell(Texture2D image, int cellSize, int cellIndex)
        {
            this.image = image;
            this.cellSize = cellSize;
            this.cellIndex = cellIndex;
        }



        private Rectangle renderCell()
        {
            int Columns = image.Width / cellSize;
            int Rows = image.Height / cellSize;

            int cellSearcher = 0;

            //Run through the tiles in the TileSet until you hit the index of the given celll
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    cellSearcher++;

                    if (cellSearcher == cellIndex)
                    {
                        Rectangle rendercell = new Rectangle(cellSize * col, cellSize * row, cellSize, cellSize);
                        return rendercell;
                    }
                }
            }

            throw new CellNotFoundException();
        }

        private Rectangle drawRectangle(int x, int y)
        {
            return new Rectangle(x, y, cellSize, cellSize);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(image, drawRectangle((int)position.X, (int)position.Y), renderCell(), Color.White);
        }

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;
using System;

namespace SolStandard.Utility
{
    public class TileCell : IRenderable
    {
        private ITexture2D image;
        private int cellSize;
        private int cellIndex;

        public TileCell(ITexture2D image, int cellSize, int cellIndex)
        {
            this.image = image;
            this.cellSize = cellSize;
            this.cellIndex = cellIndex;
        }

        private Rectangle RenderCell()
        {
            int Columns = image.GetWidth() / cellSize;
            int Rows = image.GetHeight() / cellSize;

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

        private Rectangle DrawRectangle(int x, int y)
        {
            return new Rectangle(x, y, cellSize, cellSize);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(image.GetTexture2D(), DrawRectangle((int)position.X, (int)position.Y), RenderCell(), Color.White);
        }

        public override string ToString()
        {
            return "TileCell: <CellIndex," + cellIndex + "><CellSize," + cellSize + ">";
        }
    }
}

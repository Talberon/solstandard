using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility
{
    public class TileCell : IRenderable
    {
        private readonly ITexture2D image;
        private readonly int cellSize;
        private readonly int cellIndex;
        private readonly Color color;

        public TileCell(ITexture2D image, int cellSize, int cellIndex)
        {
            this.image = image;
            this.cellSize = cellSize;
            this.cellIndex = cellIndex;
            this.color = Color.White;
        }
        
        public TileCell(ITexture2D image, int cellSize, int cellIndex, Color color)
        {
            this.image = image;
            this.cellSize = cellSize;
            this.cellIndex = cellIndex;
            this.color = color;
        }

        private Rectangle RenderCell()
        {
            int columns = image.GetWidth() / cellSize;
            int rows = image.GetHeight() / cellSize;

            int cellSearcher = 0;

            //Run through the tiles in the TileSet until you hit the index of the given celll
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
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

        public int GetHeight()
        {
            return cellSize;
        }

        public int GetWidth()
        {
            return cellSize;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(image.GetTexture2D(), DrawRectangle((int) position.X, (int) position.Y), RenderCell(),
                color);
        }

        public override string ToString()
        {
            return "TileCell: <CellIndex," + cellIndex + "><CellSize," + cellSize + ">";
        }
    }
}
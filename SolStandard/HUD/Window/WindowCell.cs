using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window
{
    public class WindowCell
    {
        private readonly int cellSize;
        private readonly int cellIndex;
        private readonly Color color;
        private readonly Vector2 coordinates;

        public WindowCell(int cellSize, int cellIndex, Color color, Vector2 coordinates)
        {
            this.cellSize = cellSize;
            this.cellIndex = cellIndex;
            this.color = color;
            this.coordinates = coordinates;
        }
        
        public int GetHeight()
        {
            return cellSize;
        }

        public int GetWidth()
        {
            return cellSize;
        }

        private Rectangle RenderCell(ref ITexture2D image)
        {
            int columns = image.GetWidth() / cellSize;
            int rows = image.GetHeight() / cellSize;

            int cellSearcher = 0;

            //Run through the tiles in the TileSet until you hit the index of the given cell
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

        public override string ToString()
        {
            return "WindowCell: <CellIndex," + cellIndex + "><CellSize," + cellSize + ">";
        }
        
        public void Draw(SpriteBatch spriteBatch, ref ITexture2D image, Vector2 offset)
        {
            Vector2 relativePosition = new Vector2(coordinates.X + offset.X, coordinates.Y + offset.Y);
            spriteBatch.Draw(image.GetTexture2D(), DrawRectangle((int) relativePosition.X, (int) relativePosition.Y), RenderCell(ref image),
                color);
        }
    }
}
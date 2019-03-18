using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Window
{
    public class WindowCell
    {
        private readonly int cellSize;
        private readonly int cellIndex;
        private readonly Vector2 coordinates;
        private readonly ITexture2D windowTexture;
        private readonly Rectangle renderCell;
        private Rectangle positionRectangle;

        public WindowCell(int cellSize, int cellIndex, Vector2 coordinates, ITexture2D windowTexture)
        {
            this.cellSize = cellSize;
            this.cellIndex = cellIndex;
            this.coordinates = coordinates;
            this.windowTexture = windowTexture;
            renderCell = RenderCell(windowTexture);
            SetRelativePosition(Vector2.Zero);
        }

        public int Height
        {
            get { return cellSize; }
        }

        public int Width
        {
            get { return cellSize; }
        }

        private Rectangle RenderCell(ITexture2D image)
        {
            int columns = image.Width / cellSize;
            int rows = image.Height / cellSize;

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

        public void SetRelativePosition(Vector2 origin)
        {
            Vector2 relativePosition = new Vector2(coordinates.X + origin.X, coordinates.Y + origin.Y);
            positionRectangle = DrawRectangle((int) relativePosition.X, (int) relativePosition.Y);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(windowTexture.MonoGameTexture, positionRectangle, renderCell, color);
        }
    }
}
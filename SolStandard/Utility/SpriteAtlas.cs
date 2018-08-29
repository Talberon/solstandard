using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility
{
    public class SpriteAtlas : IRenderable
    {
        private readonly ITexture2D image;
        private readonly int cellSize;
        public int CellIndex { get; set; }

        //TODO Decide if cellSize should continue to assume a square or take two dimensions
        public SpriteAtlas(ITexture2D image, int cellSize, int cellIndex)
        {
            this.image = image;
            this.cellSize = cellSize;
            CellIndex = cellIndex;
        }

        private Rectangle SourceRectangle()
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

                    if (cellSearcher == CellIndex)
                    {
                        Rectangle rendercell = new Rectangle(cellSize * col, cellSize * row, cellSize, cellSize);
                        return rendercell;
                    }
                }
            }

            throw new CellNotFoundException();
        }

        private Rectangle DestinationRectangle(int x, int y)
        {
            return new Rectangle(x, y, cellSize, cellSize);
        }

        public int Height
        {
            get { return cellSize; }
        }

        public int Width
        {
            get { return cellSize; }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(image.MonoGameTexture, DestinationRectangle((int) position.X, (int) position.Y),
                SourceRectangle(), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.Draw(image.MonoGameTexture, DestinationRectangle((int) position.X, (int) position.Y),
                SourceRectangle(), color);
        }

        public override string ToString()
        {
            return "SpriteAtlas: <Name," + image.Name + "><CellIndex," + CellIndex + "><CellSize," + cellSize + ">";
        }
    }
}
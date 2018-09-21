using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility
{
    public class SpriteAtlas : IRenderable
    {
        private readonly ITexture2D image;
        private readonly Vector2 cellSize;
        private readonly Vector2 renderSize;
        public int CellIndex { get; set; }
        public Color RenderColor { get; set; }

        public SpriteAtlas(ITexture2D image, Vector2 cellSize, Vector2 renderSize, int cellIndex, Color renderColor)
        {
            if (cellIndex < 1) throw new InvalidCellIndexException();

            this.image = image;
            this.cellSize = cellSize;
            this.renderSize = renderSize;
            CellIndex = cellIndex;
            RenderColor = renderColor;
        }
        
        public SpriteAtlas(ITexture2D image, Vector2 cellSize, Vector2 renderSize, int cellIndex) : this(image, cellSize,
            renderSize, cellIndex, Color.White)
        {
        }

        public SpriteAtlas(ITexture2D image, Vector2 cellSize, int cellIndex, Color renderColor) : this(image, cellSize,
            cellSize, cellIndex, renderColor)
        {
        }

        public SpriteAtlas(ITexture2D image, Vector2 cellSize, int cellIndex) : this(image, cellSize, cellIndex,
            Color.White)
        {
        }

        private Rectangle SourceRectangle()
        {
            int columns = image.Width / (int) cellSize.X;
            int rows = image.Height / (int) cellSize.Y;

            int cellSearcher = 0;

            //Run through the tiles in the TileSet until you hit the index of the given cell
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    cellSearcher++;

                    if (cellSearcher == CellIndex)
                    {
                        Rectangle rendercell = new Rectangle((int) (cellSize.X * col), (int) (cellSize.Y * row),
                            (int) cellSize.X, (int) cellSize.Y);
                        return rendercell;
                    }
                }
            }

            throw new CellNotFoundException();
        }

        private Rectangle DestinationRectangle(int x, int y)
        {
            return new Rectangle(x, y, (int) renderSize.X, (int) renderSize.Y);
        }

        public int Height
        {
            get { return (int) renderSize.Y; }
        }

        public int Width
        {
            get { return (int) renderSize.X; }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, RenderColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            spriteBatch.Draw(image.MonoGameTexture, DestinationRectangle((int) position.X, (int) position.Y),
                SourceRectangle(), colorOverride);
        }

        public override string ToString()
        {
            return "SpriteAtlas: <Name," + image.Name + "><CellIndex," + CellIndex + "><CellSize," + cellSize + ">";
        }
    }
}
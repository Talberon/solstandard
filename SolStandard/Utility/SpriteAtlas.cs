using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility
{
    public class SpriteAtlas : IRenderable, IResizable, IRotatable
    {
        private readonly ITexture2D image;
        private readonly Vector2 cellSize;
        private readonly Vector2 renderSize;
        public Color DefaultColor { get; set; }
        private Rectangle sourceRectangle;
        private int cellIndex;
        public float RotationInDegrees { get; set; }
        public Vector2 RotationOrigin { get; set; }

        public SpriteAtlas(ITexture2D image, Vector2 cellSize, Vector2 renderSize, int cellIndex, Color color,
            float rotationInDegrees = 0.0f)
        {
            if (cellIndex < 0) throw new InvalidCellIndexException();

            this.image = image;
            this.cellSize = cellSize;
            this.renderSize = renderSize;
            RotationInDegrees = rotationInDegrees;
            DefaultColor = color;
            RotationOrigin = Vector2.Zero;
            SetCellIndex(cellIndex);
        }

        public SpriteAtlas(ITexture2D image, Vector2 cellSize, Vector2 renderSize, int cellIndex = 0) : this(image,
            cellSize, renderSize, cellIndex, Color.White)
        {
        }

        public SpriteAtlas(ITexture2D image, Vector2 cellSize, int cellIndex, Color renderColor) : this(image, cellSize,
            cellSize, cellIndex, renderColor)
        {
        }

        public SpriteAtlas(ITexture2D image, Vector2 cellSize, int cellIndex = 0) : this(image, cellSize, cellIndex,
            Color.White)
        {
        }

        public void SetCellIndex(int index)
        {
            cellIndex = index;
            sourceRectangle = CalculateSourceRectangle(image, cellSize, index);
        }

        private static Rectangle CalculateSourceRectangle(ITexture2D image, Vector2 cellSize, int cellIndex)
        {
            int columns = image.Width / (int) cellSize.X;
            int rows = image.Height / (int) cellSize.Y;

            int cellSearcher = 0;

            //Run through the tiles in the TileSet until you hit the index of the given cell
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (cellSearcher == cellIndex)
                    {
                        var rendercell = new Rectangle((int) (cellSize.X * col), (int) (cellSize.Y * row),
                            (int) cellSize.X, (int) cellSize.Y);
                        return rendercell;
                    }

                    cellSearcher++;
                }
            }

            throw new CellNotFoundException();
        }

        private Rectangle DestinationRectangle(int x, int y)
        {
            return new Rectangle(x, y, (int) renderSize.X, (int) renderSize.Y);
        }

        public int Height => (int) renderSize.Y;

        public int Width => (int) renderSize.X;

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            (float x, float y) = position;
            spriteBatch.Draw(
                image.MonoGameTexture,
                DestinationRectangle((int) x, (int) y),
                sourceRectangle,
                colorOverride,
                RotationInDegrees,
                RotationOrigin,
                SpriteEffects.None,
                1
            );
        }

        public override string ToString()
        {
            return "SpriteAtlas: <Name," + image.Name + "><cellIndex," + cellIndex + "><CellSize," + cellSize + ">";
        }

        public IRenderable Resize(Vector2 newSize)
        {
            return new SpriteAtlas(image, cellSize, newSize, cellIndex, DefaultColor, RotationInDegrees);
        }

        public IRenderable Clone()
        {
            return new SpriteAtlas(image, cellSize, renderSize, cellIndex, DefaultColor, RotationInDegrees);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.NeoGFX.GUI;
using SolStandard.NeoUtility.Monogame.Interfaces;
using SolStandard.Utility.Exceptions;

namespace SolStandard.NeoGFX.Graphics
{
    public class SpriteAtlas : IPositionedNeoRenderable, IWindowContent
    {
        public readonly ITexture2D Texture;
        private readonly Vector2 cellSize;
        private Vector2 renderSize;
        private readonly int mapLayer;
        public Color DefaultColor { get; set; }
        public Rectangle SourceRectangle { get; private set; }
        private int cellIndex;
        private float RotationInDegrees { get; set; }
        private Vector2 RotationOrigin { get; set; }
        public Vector2 TopLeftPoint { get; set; }

        public Vector2 Size => new Vector2(Width, Height);

        public float Height
        {
            get => renderSize.Y;
            set => renderSize.Y = value;
        }

        public float Width
        {
            get => renderSize.X;
            set => renderSize.X = value;
        }

        public Color AverageColor => Texture.GetAverageColor(SourceRectangle);

        public SpriteAtlas(ITexture2D texture, Vector2 cellSize, Vector2 renderSize, int cellIndex,
            Vector2 position, Color color, int layerDepth = 1, float rotationInDegrees = 0.0f)
        {
            if (cellIndex < 0) throw new InvalidCellIndexException();

            Texture = texture;
            this.cellSize = cellSize;
            this.renderSize = renderSize;
            this.mapLayer = layerDepth;
            RotationInDegrees = rotationInDegrees;
            DefaultColor = color;
            TopLeftPoint = position;
            RotationOrigin = Vector2.Zero;
            SetCellFrameIndex(cellIndex);
        }

        public SpriteAtlas(ITexture2D texture, Vector2 cellSize, Vector2 renderSize, int cellIndex, Vector2 position,
            int layerDepth) :
            this(texture, cellSize, renderSize, cellIndex, position, Color.White, layerDepth)
        {
        }

        public SpriteAtlas(ITexture2D texture, Vector2 cellSize, Vector2 renderSize, int cellIndex, int layerDepth) :
            this(texture, cellSize, renderSize, cellIndex, Vector2.Zero, Color.White, layerDepth)
        {
        }

        public void RotateClockwise(float degrees)
        {
            RotationOrigin = cellSize / 2;
            RotationInDegrees += degrees;
        }

        public void RotateCounterClockwise(float degrees)
        {
            RotationOrigin = cellSize / 2;
            RotationInDegrees -= degrees;
        }

        private void SetCellFrameIndex(int index)
        {
            cellIndex = index;
            SourceRectangle = CalculateSourceRectangle(Texture, cellSize, index);
        }

        private static Rectangle CalculateSourceRectangle(ITexture2D image, Vector2 cellSize, int cellIndex)
        {
            int columns = image.Width / (int) cellSize.X;
            int rows = image.Height / (int) cellSize.Y;

            int totalCells = columns * rows;
            if (cellIndex > totalCells || cellIndex < 0) throw new CellNotFoundException();

            int renderRow = cellIndex / columns;
            int renderColumn = cellIndex % columns;

            var rendercell = new Rectangle(
                (int) (cellSize.X * renderColumn),
                (int) (cellSize.Y * renderRow),
                (int) cellSize.X,
                (int) cellSize.Y
            );

            return rendercell;
        }

        public void Update(GameTime gameTime)
        {
            //Do nothing.
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, TopLeftPoint);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 coordinates)
        {
            (float x, float y) = coordinates;
            spriteBatch.Draw(
                Texture.MonoGameTexture,
                DestinationRectangle((int) x, (int) y),
                SourceRectangle,
                DefaultColor,
                RotationInDegrees,
                RotationOrigin,
                SpriteEffects.None,
                SpriteBatchExtensions.GetLayerDepth(TopLeftPoint.Y, Height, mapLayer)
            );
        }

        private Rectangle DestinationRectangle(int x, int y)
        {
            return new Rectangle(x, y, (int) renderSize.X, (int) renderSize.Y);
        }

        public SpriteAtlas FitWithinSize(Vector2 maxSize)
        {
            SpriteAtlas clone = Clone();

            (float originalX, float originalY) = clone.renderSize;
            (float maxX, float maxY) = maxSize;

            if (originalX > originalY)
            {
                float scaledWidth = originalX * maxY / originalY;
                clone.renderSize = new Vector2(scaledWidth, maxY);
            }
            else if (originalX < originalY)
            {
                float scaledHeight = originalY * maxX / originalX;
                clone.renderSize = new Vector2(maxX, scaledHeight);
            }
            else
            {
                float scaledWidth = originalX * maxY / originalY;
                clone.renderSize = new Vector2(scaledWidth, maxY);
            }

            return clone;
        }

        public SpriteAtlas Resize(float factor)
        {
            SpriteAtlas clone = Clone();
            clone.renderSize *= factor;
            return clone;
        }

        private SpriteAtlas Clone()
        {
            return new SpriteAtlas(Texture, cellSize, renderSize, cellIndex, TopLeftPoint, DefaultColor, mapLayer,
                RotationInDegrees);
        }

        public override string ToString()
        {
            return
                $"SpriteAtlas: <Name,{Texture.Name}><cellIndex,{cellIndex}><CellSize,{cellSize}><LayerDepth,{SpriteBatchExtensions.GetLayerDepth(TopLeftPoint.Y, Height, mapLayer)}>";
        }
    }
}
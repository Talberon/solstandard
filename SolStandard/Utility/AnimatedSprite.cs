using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility
{
    public class AnimatedSprite : IRenderable
    {
        private readonly ITexture2D spriteMap;
        private readonly int cellSize;
        private Vector2 renderSize;
        private readonly Vector2 spriteFrameCount;
        private int currentRow;
        private int currentColumn;
        private int frameDelayCounter;
        protected int FrameDelay { get; set; }
        protected readonly int DefaultFrameDelay;
        private readonly bool reversible;
        private bool reversing;

        public AnimatedSprite(ITexture2D spriteMap, int cellSize, Vector2 renderSize, int frameDelay, bool reversible)
        {
            this.spriteMap = spriteMap;
            this.cellSize = cellSize;
            this.reversible = reversible;
            DefaultFrameDelay = frameDelay;
            FrameDelay = frameDelay;
            frameDelayCounter = 0;
            currentRow = 0;
            currentColumn = 0;
            reversing = false;
            spriteFrameCount = CalculateSpriteFrameCount();
            this.renderSize = renderSize;
        }

        public AnimatedSprite(ITexture2D spriteMap, int cellSize, int frameDelay, bool reversible) : this(spriteMap,
            cellSize, new Vector2(cellSize), frameDelay, reversible)
        {
        }


        public void SetSpriteCell(int spriteMapColumn, int spriteMapRow)
        {
            currentColumn = spriteMapColumn;
            currentRow = spriteMapRow;
        }

        private Vector2 CalculateSpriteFrameCount()
        {
            float columns = (float) spriteMap.Width / cellSize;
            float rows = (float) spriteMap.Width / cellSize;

            return new Vector2(columns, rows);
        }


        private void UpdateFrame()
        {
            if (frameDelayCounter % FrameDelay == 0)
            {
                frameDelayCounter = 0;

                if (currentColumn < spriteFrameCount.X - 1)
                {
                    currentColumn++;
                }
                else
                {
                    currentColumn = 0;
                }
            }

            frameDelayCounter++;
        }

        private void UpdateFrameReversible()
        {
            if (frameDelayCounter % FrameDelay == 0)
            {
                frameDelayCounter = 0;

                if (currentColumn < spriteFrameCount.X - 1 && !reversing)
                {
                    currentColumn++;
                }
                else if (reversing && currentColumn > 0)
                {
                    currentColumn--;
                }

                if (currentColumn >= spriteFrameCount.X - 1 || reversing && currentColumn <= 0)
                {
                    reversing = !reversing;
                }
            }

            frameDelayCounter++;
        }

        public int Height
        {
            get { return (int) renderSize.Y; }
        }

        public int Width
        {
            get { return (int) renderSize.X; }
        }

        public void Resize(Vector2 newSize)
        {
            renderSize = newSize;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            if (reversible)
            {
                UpdateFrameReversible();
            }
            else
            {
                UpdateFrame();
            }

            spriteBatch.Draw(spriteMap.MonoGameTexture,
                RenderRectangle(position), CurrentCell(), colorOverride);
        }

        private Rectangle RenderRectangle(Vector2 position)
        {
            return new Rectangle((int) position.X, (int) position.Y, (int) renderSize.X, (int) renderSize.Y);
        }

        private Rectangle CurrentCell()
        {
            Rectangle rendercell = new Rectangle(cellSize * currentColumn, cellSize * currentRow, cellSize, cellSize);
            return rendercell;
        }

        public AnimatedSprite Clone()
        {
            return new AnimatedSprite(spriteMap, cellSize, renderSize, FrameDelay, reversible);
        }
    }
}
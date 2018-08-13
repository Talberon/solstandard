using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility
{
    public class AnimatedSprite : IRenderable
    {
        private readonly ITexture2D spriteMap;
        private readonly int cellSize;
        private readonly Vector2 spriteFrameCount;
        private int currentRow;
        private int currentColumn;
        private int frameDelayCounter;
        private readonly int frameDelay;
        private readonly bool reversible;
        private bool reversing;

        public AnimatedSprite(ITexture2D spriteMap, int cellSize, int frameDelay, bool reversible)
        {
            this.spriteMap = spriteMap;
            this.cellSize = cellSize;
            this.frameDelay = frameDelay;
            this.reversible = reversible;
            frameDelayCounter = 0;
            currentRow = 0;
            currentColumn = 0;
            reversing = false;
            spriteFrameCount = CalculateSpriteFrameCount();
        }

        public void SetSpriteAnimation(int spriteMapRow)
        {
            currentRow = spriteMapRow;
        }

        private Vector2 CalculateSpriteFrameCount()
        {
            float columns = spriteMap.GetWidth() / cellSize;
            float rows = spriteMap.GetWidth() / cellSize;

            return new Vector2(columns, rows);
        }

        private Rectangle RenderCell()
        {
            Rectangle rendercell = new Rectangle(cellSize * currentColumn, cellSize * currentRow, cellSize, cellSize);
            return rendercell;
        }


        private void UpdateFrame()
        {
            if (frameDelayCounter % frameDelay == 0)
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
            if (frameDelayCounter % frameDelay == 0)
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
            if (reversible)
            {
                UpdateFrameReversible();
            }
            else
            {
                UpdateFrame();
            }

            spriteBatch.Draw(spriteMap.GetTexture2D(),
                new Rectangle((int) position.X, (int) position.Y, cellSize, cellSize), RenderCell(), Color.White);
        }
    }
}
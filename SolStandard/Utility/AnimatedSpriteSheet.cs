using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility
{
    public class AnimatedSpriteSheet : IRenderable, IResizable
    {
        public readonly ITexture2D SpriteMap;
        public readonly int CellSize;
        public readonly Vector2 RenderSize;
        protected readonly int SpriteFrameCount;
        private int currentRow;
        protected int CurrentColumn { get; private set; }
        protected int FrameDelayCounter { get; private set; }
        public int FrameDelay { get; protected set; }
        protected readonly int DefaultFrameDelay;
        public readonly bool Reversible;
        private bool reversing;
        public Color DefaultColor { get; set; }
        public bool IsFlipped { get; private set; }
        private bool isPaused;
        private bool playingOnce;

        public AnimatedSpriteSheet(ITexture2D spriteMap, int cellSize, Vector2 renderSize, int frameDelay,
            bool reversible, Color color, bool isFlipped = false, int currentRow = 0)
        {
            SpriteMap = spriteMap;
            CellSize = cellSize;
            Reversible = reversible;
            DefaultFrameDelay = frameDelay;
            FrameDelay = frameDelay;
            FrameDelayCounter = 0;
            this.currentRow = currentRow;
            CurrentColumn = 0;
            reversing = false;
            SpriteFrameCount = CalculateSpriteFrameCount();
            RenderSize = renderSize;
            DefaultColor = color;
            IsFlipped = isFlipped;
        }

        public AnimatedSpriteSheet(ITexture2D spriteMap, int cellSize, int frameDelay, bool reversible,
            bool isFlipped = false) : this(
            spriteMap, cellSize, new Vector2(cellSize), frameDelay, reversible, Color.White, isFlipped)
        {
        }


        public void SetSpriteCell(int spriteMapColumn, int spriteMapRow)
        {
            CurrentColumn = spriteMapColumn;
            currentRow = spriteMapRow;
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Play()
        {
            isPaused = false;
            playingOnce = false;
        }

        public void PlayOnce()
        {
            isPaused = false;
            SetSpriteCell(0, currentRow);
            playingOnce = true;
        }

        private int CalculateSpriteFrameCount()
        {
            float columns = (float) SpriteMap.Width / CellSize;
            return Convert.ToInt32(columns);
        }

        protected void ResetAnimation()
        {
            CurrentColumn = 0;
            FrameDelayCounter = 0;
        }

        private void UpdateFrame()
        {
            if (FrameDelayCounter % FrameDelay == 0)
            {
                FrameDelayCounter = 0;

                if (CurrentColumn < SpriteFrameCount - 1)
                {
                    CurrentColumn++;
                }
                else
                {
                    CurrentColumn = 0;
                    PauseIfPlayingOnce();
                }
            }

            FrameDelayCounter++;
        }

        private void UpdateFrameReversible()
        {
            if (FrameDelayCounter % FrameDelay == 0)
            {
                FrameDelayCounter = 0;

                if (CurrentColumn < SpriteFrameCount - 1 && !reversing)
                {
                    CurrentColumn++;
                }
                else if (reversing && CurrentColumn > 0)
                {
                    CurrentColumn--;
                }

                if (CurrentColumn >= SpriteFrameCount - 1 || reversing && CurrentColumn <= 0)
                {
                    reversing = !reversing;
                    PauseIfPlayingOnce();
                }
            }

            FrameDelayCounter++;
        }

        private void PauseIfPlayingOnce()
        {
            if (!playingOnce) return;
            isPaused = true;
            playingOnce = false;
        }

        public int Height => (int) RenderSize.Y;
        public int Width => (int) RenderSize.X;

        public void Flip()
        {
            IsFlipped = !IsFlipped;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            if (!isPaused)
            {
                if (Reversible)
                {
                    UpdateFrameReversible();
                }
                else
                {
                    UpdateFrame();
                }
            }

            spriteBatch.Draw(SpriteMap.MonoGameTexture, RenderRectangle(position), CurrentCell(), colorOverride, 0f,
                Vector2.Zero, IsFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        private Rectangle RenderRectangle(Vector2 position)
        {
            return new Rectangle((int) position.X, (int) position.Y, (int) RenderSize.X, (int) RenderSize.Y);
        }

        private Rectangle CurrentCell()
        {
            var rendercell = new Rectangle(CellSize * CurrentColumn, CellSize * currentRow, CellSize, CellSize);
            return rendercell;
        }

        public virtual IRenderable Resize(Vector2 newSize)
        {
            return new AnimatedSpriteSheet(SpriteMap, CellSize, newSize, FrameDelay, Reversible, DefaultColor,
                IsFlipped, currentRow);
        }

        public virtual IRenderable Clone()
        {
            return new AnimatedSpriteSheet(SpriteMap, CellSize, RenderSize, FrameDelay, Reversible, DefaultColor,
                IsFlipped, currentRow);
        }
    }
}
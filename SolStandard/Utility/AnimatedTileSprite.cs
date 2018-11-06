using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility
{
    public class AnimatedTileSprite : IRenderable
    {
        private const int DefaultFrameDelay = 10;

        private readonly Vector2 renderSize;
        private readonly int frameDelay;
        private int frameDelayCounter;
        private int currentFrameIndex;

        private readonly List<int> frameIds;
        private readonly List<SpriteAtlas> frameSprites;

        public AnimatedTileSprite(ITexture2D tileMapTexture, List<int> frameIds, Vector2 cellSize,
            Vector2 renderSize, int frameDelay = DefaultFrameDelay)
        {
            this.frameIds = frameIds;
            this.renderSize = renderSize;
            this.frameDelay = frameDelay;
            currentFrameIndex = 0;
            frameSprites = BuildFrames(tileMapTexture, this.frameIds, cellSize, renderSize);
        }

        public AnimatedTileSprite(ITexture2D tileMapTexture, List<int> frameIds, Vector2 renderSize)
            : this(tileMapTexture, frameIds, new Vector2(GameDriver.CellSize), renderSize)
        {
        }

        private static List<SpriteAtlas> BuildFrames(ITexture2D tileMapTexture, IEnumerable<int> frameIndexes,
            Vector2 cellSize, Vector2 renderSize)
        {
            return frameIndexes.Select(index => new SpriteAtlas(tileMapTexture, cellSize, renderSize, index)).ToList();
        }

        private void UpdateFrame()
        {
            if (frameDelayCounter % frameDelay == 0)
            {
                frameDelayCounter = 0;

                if (currentFrameIndex < frameIds.Count - 1)
                {
                    currentFrameIndex++;
                }
                else
                {
                    currentFrameIndex = 0;
                }
            }

            frameDelayCounter++;
        }

        public int Height
        {
            get { return Convert.ToInt32(renderSize.Y); }
        }

        public int Width
        {
            get { return Convert.ToInt32(renderSize.X); }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            UpdateFrame();
            frameSprites[currentFrameIndex].Draw(spriteBatch, position, colorOverride);
        }
    }
}
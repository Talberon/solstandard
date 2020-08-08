using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.NeoUtility.Monogame.Interfaces;

namespace SolStandard.NeoGFX.Graphics
{
    public class AnimatedTileSprite : IPositionedRenderable
    {
        private readonly int millisDelay;
        private TimeSpan frameTimeRemaining;
        private int currentFrameIndex;

        private readonly List<int> frameIds;
        private readonly List<SpriteAtlas> frameSprites;

        public float Height => frameSprites.First().Height;
        public float Width => frameSprites.First().Width;

        public Vector2 TopLeftPoint
        {
            get => frameSprites.First().TopLeftPoint;
            set
            {
                foreach (SpriteAtlas sprite in frameSprites)
                {
                    sprite.TopLeftPoint = value;
                }
            }
        }

        public AnimatedTileSprite(ITexture2D tileMapTexture, List<int> frameIds, Vector2 cellSize,
            Vector2 renderSize, Vector2 position, int millisDelay, int layerDepth)
        {
            this.frameIds = frameIds;
            this.millisDelay = millisDelay;
            currentFrameIndex = 0;
            frameSprites = BuildFrames(tileMapTexture, this.frameIds, cellSize, renderSize, position, layerDepth);
            frameTimeRemaining = TimeSpan.FromMilliseconds(millisDelay);
        }

        private static List<SpriteAtlas> BuildFrames(ITexture2D tileMapTexture, IEnumerable<int> frameIndexes,
            Vector2 cellSize, Vector2 renderSize, Vector2 position, int layerDepth)
        {
            return frameIndexes
                .Select(cellIndex => new SpriteAtlas(tileMapTexture, cellSize, renderSize, cellIndex, position, layerDepth))
                .ToList();
        }

        private void UpdateFrame(GameTime gameTime)
        {
            frameTimeRemaining -= gameTime.ElapsedGameTime;

            if (frameTimeRemaining >= TimeSpan.Zero) return;

            TimeSpan leftoverTime = frameTimeRemaining * -1;
            frameTimeRemaining = TimeSpan.Zero - leftoverTime + TimeSpan.FromMilliseconds(millisDelay);

            if (currentFrameIndex < frameIds.Count - 1)
            {
                currentFrameIndex++;
            }
            else
            {
                currentFrameIndex = 0;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (SpriteAtlas sprite in frameSprites)
            {
                sprite.Update(gameTime);
            }

            UpdateFrame(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            frameSprites[currentFrameIndex].Draw(spriteBatch);
        }
    }
}
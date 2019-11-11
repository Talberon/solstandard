using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Utility
{
    public class RotatingSprite : IRenderable
    {
        public enum RotationDirection
        {
            Clockwise,
            Counterclockwise
        }

        public int Height => sprite.Height;
        public int Width => sprite.Width;

        public Color DefaultColor
        {
            get => sprite.DefaultColor;
            set => sprite.DefaultColor = value;
        }

        private readonly SpriteAtlas sprite;
        private readonly float rotationSpeedInDegreesPerFrame;
        private readonly RotationDirection direction;
        private readonly Vector2 drawOffset;

        public RotatingSprite(SpriteAtlas sprite, float rotationSpeedInDegreesPerFrame, RotationDirection direction)
        {
            this.sprite = sprite;
            drawOffset = HalfSpriteSize(this.sprite);
            this.sprite.RotationOrigin = drawOffset;
            this.rotationSpeedInDegreesPerFrame = rotationSpeedInDegreesPerFrame;
            this.direction = direction;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            sprite.RotationInDegrees += direction switch
            {
                RotationDirection.Clockwise => rotationSpeedInDegreesPerFrame,
                RotationDirection.Counterclockwise => -rotationSpeedInDegreesPerFrame,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Direction not supported")
            };

            sprite.Draw(spriteBatch, drawOffset + position, colorOverride);
        }

        public IRenderable Clone()
        {
            return new RotatingSprite(sprite, rotationSpeedInDegreesPerFrame, direction);
        }

        private static Vector2 HalfSpriteSize(IRenderable sprite)
        {
            return new Vector2(sprite.Width, sprite.Height) / 2;
        }
    }
}
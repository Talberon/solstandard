using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.HUD.Directions;
using SolStandard.Utility.HUD.Juice;

namespace SolStandard.Utility.HUD.Neo
{
    public class ScrollingBackground : IRenderable
    {
        public int Width => sprite.Width;
        public int Height => sprite.Height;

        private readonly Vector2 startPoint;
        public Vector2 TopLeftPoint { get; set; }
        private Vector2 offset;

        private readonly SpriteAtlas sprite;
        private readonly IntercardinalDirection scrollDirection;
        private readonly float scrollSpeed;

        private readonly JuiceBox juiceBox;

        public ScrollingBackground(SpriteAtlas sprite, IntercardinalDirection scrollDirection, float scrollSpeed, float fadeSpeed = 0.99f)
        {
            this.sprite = sprite;
            this.scrollDirection = scrollDirection;
            this.scrollSpeed = scrollSpeed;
            juiceBox = new JuiceBox.Builder(fadeSpeed).WithColorShifting(sprite.DefaultColor).Build();

            switch (scrollDirection)
            {
                case IntercardinalDirection.NorthWest:
                    startPoint = new Vector2(sprite.Width, sprite.Height);
                    break;
                case IntercardinalDirection.North:
                    startPoint = new Vector2(0, sprite.Height);
                    break;
                case IntercardinalDirection.NorthEast:
                    startPoint = new Vector2(0, sprite.Height);
                    break;
                case IntercardinalDirection.West:
                    startPoint = new Vector2(sprite.Width, 0);
                    break;
                case IntercardinalDirection.East:
                    startPoint = Vector2.Zero;
                    break;
                case IntercardinalDirection.SouthWest:
                    startPoint = new Vector2(sprite.Width, 0);
                    break;
                case IntercardinalDirection.South:
                    startPoint = Vector2.Zero;
                    break;
                case IntercardinalDirection.SouthEast:
                    startPoint = Vector2.Zero;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scrollDirection), scrollDirection, null);
            }

            offset = Vector2.Zero;
        }

        public void HueShiftTowards(Color targetColor)
        {
            juiceBox.HueShiftTo(targetColor);
        }

        public void Update(GameTime gameTime)
        {
            juiceBox.Update();

            offset -= scrollSpeed * scrollDirection.ToVector();

            if (offset.AbsX() >= sprite.Width)
            {
                offset.X = 0;
            }

            if (offset.AbsY() >= sprite.Height)
            {
                offset.Y = 0;
            }

            sprite.DefaultColor = juiceBox.CurrentColor;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, startPoint + offset);
            sprite.Draw(spriteBatch, startPoint + offset + new Vector2(sprite.Width, 0));
            sprite.Draw(spriteBatch, startPoint + offset + new Vector2(0, sprite.Height));
            sprite.Draw(spriteBatch, startPoint + offset + new Vector2(sprite.Width, sprite.Height));
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 _)
        {
            Draw(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 _, Color _1)
        {
            Draw(spriteBatch);
        }
        
        
        public Color DefaultColor { get; set; } //Ignore
        public IRenderable Clone()
        {
            throw new NotImplementedException();
        }
    }
}
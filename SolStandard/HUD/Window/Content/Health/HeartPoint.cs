﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class HeartPoint : IResourcePoint
    {
        public bool Active { get; set; }
        public Color DefaultColor { get; set; }
        private SpriteAtlas activeSprite;
        private SpriteAtlas inactiveSprite;

        public HeartPoint(Vector2 size)
        {
            Size = size;
            DefaultColor = Color.White;
        }

        public Vector2 Size
        {
            set
            {
                activeSprite = UnitStatistics.GetSpriteAtlas(Stats.Hp, value);
                inactiveSprite = UnitStatistics.GetSpriteAtlas(Stats.EmptyHp, value);
            }
        }

        public int Height
        {
            get { return activeSprite.Height; }
        }

        public int Width
        {
            get { return activeSprite.Width; }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            if (Active)
            {
                activeSprite.Draw(spriteBatch, position, colorOverride);
            }
            else
            {
                inactiveSprite.Draw(spriteBatch, position, colorOverride);
            }
        }

        public IRenderable Clone()
        {
            return new HeartPoint(new Vector2(Width, Height));
        }

        public override string ToString()
        {
            return "Heart: { Active=" + Active + "}";
        }
    }
}
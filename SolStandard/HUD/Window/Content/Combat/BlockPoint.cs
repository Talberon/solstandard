using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Combat
{
    public class BlockPoint : IRenderable, ICombatPoint
    {
        public bool Enabled { get; private set; }
        public Color DefaultColor { get; set; }
        private readonly SpriteAtlas pointSprite;

        private readonly int size;

        public BlockPoint(int size, Color? color)
        {
            this.size = size;
            DefaultColor = color ?? Color.White;
            Enabled = true;
            pointSprite = UnitStatistics.GetSpriteAtlas(Stats.Block, new Vector2(size));
        }

        public int Height => pointSprite.Height;

        public int Width => pointSprite.Width;

        public void Disable(Color disabledColor)
        {
            Enabled = false;
            DefaultColor = disabledColor;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, DefaultColor);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            pointSprite.Draw(spriteBatch, position, colorOverride);
        }


        public IRenderable Clone()
        {
            return new AttackPoint(size, DefaultColor);
        }
    }
}
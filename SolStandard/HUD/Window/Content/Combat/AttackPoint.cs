using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Combat
{
    public class AttackPoint : IRenderable, ICombatPoint
    {
        public bool Enabled { get; private set; }
        private Color color;
        private readonly SpriteAtlas pointSprite;

        public AttackPoint(int size)
        {
            color = Color.White;
            Enabled = true;
            pointSprite = UnitStatistics.GetSpriteAtlas(StatIcons.Atk, new Vector2(size));
        }

        public int Height
        {
            get { return pointSprite.Height; }
        }

        public int Width
        {
            get { return pointSprite.Width; }
        }

        public void Disable(Color disabledColor)
        {
            Enabled = false;
            color = disabledColor;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, color);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color colorOverride)
        {
            pointSprite.Draw(spriteBatch, position, colorOverride);
        }
    }
}
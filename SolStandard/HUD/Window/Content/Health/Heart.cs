using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class Heart : IResourcePoint
    {
        public bool Active { get; set; }
        private readonly SpriteAtlas activeSprite;
        private readonly SpriteAtlas inactiveSprite;

        public Heart(Vector2 size)
        {
            activeSprite = UnitStatistics.GetSpriteAtlas(StatIcons.Hp, size);
            inactiveSprite = UnitStatistics.GetSpriteAtlas(StatIcons.EmptyHp, size);
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
            Draw(spriteBatch, position, Color.White);
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

        public override string ToString()
        {
            return "Heart: { Active=" + Active + "}";
        }
    }
}
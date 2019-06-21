using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public class ResourcePoint : IResourcePoint
    {
        public bool Active { get; set; }
        public Color DefaultColor { get; set; }
        private SpriteAtlas activeSprite;
        private SpriteAtlas inactiveSprite;

        public ResourcePoint(Vector2 size, SpriteAtlas activeSprite, SpriteAtlas inactiveSprite)
        {
            DefaultColor = Color.White;
            this.activeSprite = activeSprite;
            this.inactiveSprite = inactiveSprite;
            Size = size;
        }

        private void ResizePips(Vector2 newSize)
        {
            activeSprite = activeSprite.Resize(newSize) as SpriteAtlas;
            inactiveSprite = inactiveSprite.Resize(newSize) as SpriteAtlas;
        }

        public Vector2 Size
        {
            set => ResizePips(value);
        }

        public int Height => activeSprite.Height;

        public int Width => activeSprite.Width;

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
            return new ResourcePoint(new Vector2(Width, Height), activeSprite, inactiveSprite);
        }

        public override string ToString()
        {
            return "Heart: { Active=" + Active + "}";
        }
    }
}
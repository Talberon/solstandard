using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Elements
{
    public abstract class MapElement
    {
        protected IRenderable Sprite;
        protected Color ElementColor = Color.White;
        public Vector2 MapCoordinates { get; set; }

        public IRenderable RenderSprite
        {
            get { return Sprite; }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, ElementColor);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color colorOverride)
        {
            Sprite.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize, colorOverride);
        }
    }
}
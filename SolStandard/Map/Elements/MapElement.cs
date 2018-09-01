using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Elements
{
    public abstract class MapElement
    {
        protected IRenderable Sprite;
        public Vector2 MapCoordinates { get; set; } //TODO Consider whether set should really be public here

        public IRenderable RenderSprite
        {
            get { return Sprite; }
        }
        
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize);
        }
        
        public virtual void Draw(SpriteBatch spriteBatch, Color color)
        {
            Sprite.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize, color);
        }
    }
}
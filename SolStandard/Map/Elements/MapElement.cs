using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Elements
{
    public abstract class MapElement
    {
        public IRenderable Sprite { get; protected set; }
        protected Vector2 MapCoordinates;

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize);
        }
        
        public virtual void Draw(SpriteBatch spriteBatch, Color color)
        {
            Sprite.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize, color);
        }
    }
}
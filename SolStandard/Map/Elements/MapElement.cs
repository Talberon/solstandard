using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Elements
{
    public abstract class MapElement
    {
        protected IRenderable Sprite;
        protected Vector2 MapCoordinates;

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize);
        }

        //TODO Decide if this really should be exposed
        public IRenderable GetSprite()
        {
            return Sprite;
        }

    }
}
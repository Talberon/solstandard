using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Objects
{
    public class MapObject
    {
        protected IRenderable Sprite;
        protected Vector2 MapCoordinates;

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize);
        }
    }
}

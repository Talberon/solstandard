using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Objects
{
    public class MapObject
    {
        protected TileCell tileCell;
        protected Vector2 mapCoordinates;

        public TileCell GetTileCell()
        {
            return tileCell;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            tileCell.Draw(spriteBatch, mapCoordinates * GameDriver.CELL_SIZE);
        }
    }
}

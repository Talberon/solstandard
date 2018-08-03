using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility;

namespace SolStandard.Map.Objects
{
    public class MapObject
    {
        protected TileCell TileCell;
        protected Vector2 MapCoordinates;

        public TileCell GetTileCell()
        {
            return TileCell;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TileCell.Draw(spriteBatch, MapCoordinates * GameDriver.CellSize);
        }
    }
}

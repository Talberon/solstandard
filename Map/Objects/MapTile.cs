using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Map.Objects
{
    public class MapTile : MapObject
    {
        /**
         * GameTile
         * Holds a texture to be rendered on the map.
         */

        public MapTile(TileCell tileCell, Vector2 mapCoordinates)
        {
            TileCell = tileCell;
            MapCoordinates = mapCoordinates;
        }

        public override string ToString()
        {
            return "Tile: {" + TileCell + "}";
        }
    }
}

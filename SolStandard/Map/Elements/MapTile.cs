using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Map.Elements
{
    public class MapTile : MapElement
    {
        /**
         * GameTile
         * Holds a texture to be rendered on the map.
         */

        public MapTile(TileCell sprite, Vector2 mapCoordinates)
        {
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
        }

        public override string ToString()
        {
            return "Tile: {" + Sprite + "}";
        }
    }
}

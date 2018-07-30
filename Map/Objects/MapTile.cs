using Microsoft.Xna.Framework;
using SolStandard.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.tileCell = tileCell;
            this.mapCoordinates = mapCoordinates;
        }

        public override string ToString()
        {
            return "Tile: {" + tileCell.ToString() + "}";
        }
    }
}

using SolStandard.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolStandard.Map.Objects
{
    class MapTile : MapObject
    {
        /**
         * GameTile
         * Holds a texture to be rendered on the map.
         */
        private TileCell tileCell;

        public MapTile(TileCell tileCell)
        {
            this.tileCell = tileCell;
        }

    }
}

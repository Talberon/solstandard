using SolStandard.Map.Objects.EntityProps;
using SolStandard.Utility;
using System.Collections.Generic;

namespace SolStandard.Map.Objects
{
    /**
     * GameObject
     * Holds a texture and certain attributes that impact how the tile can be interacted with on the game map
     */
    class MapEntity : MapObject
    {
        private TileCell tileCell;
        private List<EntityProp> entityProps;
        private string name;

        public MapEntity(string name, TileCell tileCell, List<EntityProp> entityProps)
        {
            this.name = name;
            this.tileCell = tileCell;
            this.entityProps = entityProps;
        }
    }
}

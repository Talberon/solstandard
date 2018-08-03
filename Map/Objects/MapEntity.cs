using Microsoft.Xna.Framework;
using SolStandard.Map.Objects.EntityProps;
using SolStandard.Utility;
using System.Collections.Generic;

namespace SolStandard.Map.Objects
{
    /**
     * GameObject
     * Holds a texture and certain attributes that impact how the tile can be interacted with on the game map
     */
    public class MapEntity : MapObject
    {
        private readonly List<EntityProp> entityProps;
        private readonly string name;


        public MapEntity(string name, TileCell tileCell, List<EntityProp> entityProps, Vector2 mapCoordinates)
        {
            this.name = name;
            TileCell = tileCell;
            this.entityProps = entityProps;
            MapCoordinates = mapCoordinates;
        }
        
        public override string ToString()
        {
            string output = "";

            output += "(" + name + "):{";
            output += "TileCell," + TileCell;
            output += " | ";
            output += "EntityProps," + entityProps;
            output += "}";

            return output;
        }
    }
}

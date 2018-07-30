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
        private List<EntityProp> entityProps;
        private string name;


        public MapEntity(string name, TileCell tileCell, List<EntityProp> entityProps, Vector2 mapCoordinates)
        {
            this.name = name;
            this.tileCell = tileCell;
            this.entityProps = entityProps;
            this.mapCoordinates = mapCoordinates;
        }
        
        public override string ToString()
        {
            string output = "";

            output += "(" + name + "):{";
            output += "TileCell," + tileCell.ToString();
            output += " | ";
            output += "EntityProps," + entityProps.ToString();
            output += "}";

            return output;
        }
    }
}

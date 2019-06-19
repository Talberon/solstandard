using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Map.Elements
{
    /**
     * GameObject
     * Holds a texture and certain attributes that impact how the tile can be interacted with on the game map
     */
    public class MapEntity : MapElement
    {
        private readonly string name;
        private readonly string type;

        public MapEntity(string name, string type, IRenderable spriteSheet, Vector2 mapCoordinates)
        {
            this.name = name;
            this.type = type;
            Sprite = spriteSheet;
            MapCoordinates = mapCoordinates;
        }

        public string Name
        {
            get { return name; }
        }

        public string Type
        {
            get { return type; }
        }

        public override string ToString()
        {
            string output = "";

            output += "(" + name + "):{";
            output += "TileCell," + Sprite;
            output += "}";

            return output;
        }
    }
}
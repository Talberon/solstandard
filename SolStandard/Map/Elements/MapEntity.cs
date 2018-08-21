using System.Collections.Generic;
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
        private readonly Dictionary<string,string> tiledProperties;

        public MapEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates, Dictionary<string,string> tiledProperties)
        {
            this.name = name;
            this.type = type;
            this.tiledProperties = tiledProperties;
            Sprite = sprite;
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

        public Dictionary<string,string> TiledProperties
        {
            get { return tiledProperties; }
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
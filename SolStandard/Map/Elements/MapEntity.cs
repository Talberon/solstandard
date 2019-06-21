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
        protected MapEntity(string name, string type, IRenderable spriteSheet, Vector2 mapCoordinates) :
            base(spriteSheet, mapCoordinates)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public string Type { get; }

        public override string ToString()
        {
            string output = "";

            output += "(" + Name + "):{";
            output += "TileCell," + Sprite;
            output += "}";

            return output;
        }
    }
}
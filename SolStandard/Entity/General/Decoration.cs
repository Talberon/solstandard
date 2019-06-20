using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Decoration : TerrainEntity
    {
        public Decoration(string name, string type, IRenderable sprite, Vector2 mapCoordinates) : base(name, type,
            sprite, mapCoordinates)
        {
        }
    }
}
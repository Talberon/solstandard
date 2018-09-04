using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;

namespace SolStandard.Entity.Unit
{
    public class UnitEntity : MapEntity
    {
        public UnitEntity(string name, string type, UnitSprite sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties) : base(name, type, sprite, mapCoordinates, tiledProperties)
        {
        }

        public UnitSprite UnitSprite
        {
            get { return (UnitSprite) Sprite; }
        }
    }
}
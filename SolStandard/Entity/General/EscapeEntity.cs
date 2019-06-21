using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class EscapeEntity : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; }
        public readonly bool UseableByBlue;
        public readonly bool UseableByRed;

        public EscapeEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool useableByBlue,
            bool useableByRed)
            : base(name, type, sprite, mapCoordinates)
        {
            UseableByBlue = useableByBlue;
            UseableByRed = useableByRed;
            InteractRange = new[] {0};
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new EscapeAction(this, MapCoordinates)
            };
        }
    }
}
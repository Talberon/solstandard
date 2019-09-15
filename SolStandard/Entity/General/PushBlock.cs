using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class PushBlock : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; }

        public PushBlock(string name, string type, IRenderable sprite, Vector2 mapCoordinates) :
            base(name, type, sprite, mapCoordinates)
        {
            CanMove = false;
            InteractRange = new[] {1};
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PushBlockAction(this, MapCoordinates)
            };
        }
    }
}
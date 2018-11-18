using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class PushBlock : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; private set; }

        public PushBlock(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            CanMove = false;
            InteractRange = new[] {1};
        }

        public UnitAction TileAction()
        {
            return new PushBlockAction(this, MapCoordinates);
        }
    }
}
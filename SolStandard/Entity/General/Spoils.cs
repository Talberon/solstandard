using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Skills;
using SolStandard.Entity.Unit.Skills.Terrain;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Spoils : TerrainEntity, IActionTile
    {
        public int Currency { get; private set; }
        public List<IItem> Items { get; private set; }
        public int[] Range { get; private set; }

        public Spoils(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int currency, List<IItem> items) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            Currency = currency;
            Items = items;
            Range = new[] {0, 1};
        }

        public UnitAction TileAction()
        {
            return new TakeSpoilsAction(this);
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.Utility;

namespace SolStandard.Entity.General.Item
{
    public class Relic : TerrainEntity, IItem, IActionTile
    {
        public string ItemPool { get; }
        public int[] InteractRange { get; }

        public Relic(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            string itemPool) :
            base(name, type, sprite, mapCoordinates)
        {
            ItemPool = itemPool;
            InteractRange = pickupRange;
        }

        public bool IsBroken => false;

        public IRenderable Icon => Sprite;

        public UnitAction UseAction()
        {
            return new DropGiveItemAction(this);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Relic(Name, Type, Sprite.Clone(), MapCoordinates, InteractRange, ItemPool);
        }


        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpItemAction(this, MapCoordinates)
            };
        }
    }
}
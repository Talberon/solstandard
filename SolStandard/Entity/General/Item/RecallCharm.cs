using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.Utility;

namespace SolStandard.Entity.General.Item
{
    public class RecallCharm : TerrainEntity, IItem, IActionTile
    {
        private readonly int[] deployRange;
        private readonly int usesRemaining;
        public string ItemPool { get; }
        public string RecallId { get; }
        public int[] InteractRange { get; }
        private bool recallDeployed;

        public RecallCharm(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string recallId,
            int[] pickupRange, string itemPool, int[] deployRange, int usesRemaining)
            : base(name, type, sprite, mapCoordinates)
        {
            this.deployRange = deployRange;
            this.usesRemaining = usesRemaining;
            RecallId = recallId;
            InteractRange = pickupRange;
            ItemPool = itemPool;
            recallDeployed = false;
        }

        public UnitAction UseAction()
        {
            if (recallDeployed)
            {
                return new ReturnToRecallPointAction(this);
            }

            return new DeployRecallPointAction(this, deployRange);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new RecallCharm(Name, Type, Sprite, MapCoordinates, RecallId, InteractRange, ItemPool, deployRange,
                usesRemaining);
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpItemAction(this, MapCoordinates)
            };
        }

        public void DeployRecall()
        {
            //Re-add this item to the unit's inventory to update the UseAction immediately
            GlobalContext.ActiveUnit.RemoveItemFromInventory(this);
            recallDeployed = true;
            GlobalContext.ActiveUnit.AddItemToInventory(this);
        }

        public bool IsBroken => usesRemaining < 1;

        public IRenderable Icon => Sprite;
    }
}
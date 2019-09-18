using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Magnet : TerrainEntity, IItem, IActionTile
    {
        public int[] InteractRange { get; }
        public string ItemPool { get; }
        private readonly int[] actionRange;
        private int usesRemaining;

        public Magnet(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int[] actionRange, int usesRemaining, string itemPool)
            : base(name, type, sprite, mapCoordinates)
        {
            InteractRange = pickupRange;
            this.actionRange = actionRange;
            this.usesRemaining = usesRemaining;
            ItemPool = itemPool;
        }

        public bool IsBroken => usesRemaining < 1;

        public IRenderable Icon => Sprite;

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpItemAction(this, MapCoordinates)
            };
        }

        public UnitAction UseAction()
        {
            return new MagneticPullAction(this, Sprite, actionRange);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public void DecrementRemainingUses()
        {
            usesRemaining--;
        }

        public IItem Duplicate()
        {
            return new Magnet(Name, Type, Sprite, MapCoordinates, InteractRange, actionRange, usesRemaining, ItemPool);
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                        new RenderText(AssetManager.WindowFont, ": " + $"[{string.Join(",", actionRange)}]")
                    }
                }
            );
    }
}
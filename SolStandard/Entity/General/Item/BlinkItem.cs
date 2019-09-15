using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Mage;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class BlinkItem : TerrainEntity, IItem, IActionTile
    {
        public int[] BlinkRange { get; }
        public int[] InteractRange { get; }
        public int UsesRemaining { get; }
        public string ItemPool { get; }

        public BlinkItem(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int[] blinkRange, int usesRemaining, string itemPool)
            : base(name, type, sprite, mapCoordinates)
        {
            BlinkRange = blinkRange;
            InteractRange = pickupRange;
            UsesRemaining = usesRemaining;
            ItemPool = itemPool;
        }

        public bool IsBroken => UsesRemaining < 1;

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
            return new Blink(this);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new BlinkItem(Name, Type, Sprite, MapCoordinates, InteractRange, BlinkRange, UsesRemaining,
                ItemPool);
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(new[,]
                {
                    {
                        SkillIconProvider.GetSkillIcon(SkillIcon.Blink,
                            GameDriver.CellSizeVector),
                        new RenderText(AssetManager.WindowFont,
                            "Blink Range: [" + string.Join(",", BlinkRange) + "]")
                    },
                    {
                        new RenderText(AssetManager.WindowFont,
                            "Uses Remaining: [" + UsesRemaining + "]"),
                        RenderBlank.Blank
                    }
                }
            );
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Magnet : TerrainEntity, IItem, IActionTile
    {
        public int[] InteractRange { get; private set; }
        public string ItemPool { get; private set; }
        private readonly int[] actionRange;
        private int usesRemaining;

        public Magnet(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int[] actionRange, int usesRemaining, string itemPool)
            : base(name, type, sprite, mapCoordinates, new Dictionary<string, string>())
        {
            InteractRange = pickupRange;
            this.actionRange = actionRange;
            this.usesRemaining = usesRemaining;
            ItemPool = itemPool;
        }

        public bool IsBroken
        {
            get { return usesRemaining < 1; }
        }

        public IRenderable Icon
        {
            get { return Sprite; }
        }

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

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            InfoHeader,
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            StatusIconProvider.GetStatusIcon(StatusIcon.PickupRange, new Vector2(GameDriver.CellSize)),
                            new RenderText(
                                AssetManager.WindowFont,
                                ": " + string.Format("[{0}]", string.Join(",", InteractRange))
                            )
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                            new RenderText(
                                AssetManager.WindowFont,
                                ": " + string.Format("[{0}]", string.Join(",", actionRange))
                            )
                        }
                    },
                    3
                );
            }
        }
    }
}
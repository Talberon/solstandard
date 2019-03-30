using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Key : TerrainEntity, IItem, IActionTile
    {
        public string UsedWith { get; private set; }
        public int[] InteractRange { get; private set; }
        public string ItemPool { get; private set; }

        public Key(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string usedWith, int[] range, string itemPool) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            UsedWith = usedWith;
            InteractRange = range;
            ItemPool = itemPool;
        }

        public bool IsBroken
        {
            get { return false; }
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
            return new ToggleLockAction(this);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Key(Name, Type, Sprite, MapCoordinates, TiledProperties, UsedWith, InteractRange, ItemPool);
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
                            new Window(new RenderText(AssetManager.WindowFont, "Used with: " + UsedWith),
                                InnerWindowColor),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}
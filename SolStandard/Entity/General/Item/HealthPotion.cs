using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class HealthPotion : TerrainEntity, IActionTile, IConsumable
    {
        private static readonly int[] UseRange = {0, 1};

        public int[] InteractRange { get; private set; }
        private int HPHealed { get; set; }
        public bool IsBroken { get; set; }

        public HealthPotion(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int hpHealed)
            : base(name, type, sprite, mapCoordinates, new Dictionary<string, string>())
        {
            InteractRange = pickupRange;
            HPHealed = hpHealed;
        }

        public IRenderable Icon
        {
            get { return Sprite; }
        }

        public void Consume()
        {
            IsBroken = true;
            GameContext.ActiveUnit.RemoveItemFromInventory(this);
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
            return new ConsumeRecoveryItemAction(this, HPHealed, UseRange);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new HealthPotion(Name, Type, Sprite, MapCoordinates, InteractRange, HPHealed);
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
                            new RenderText(
                                AssetManager.WindowFont,
                                "Pick-Up Range: " + string.Format("[{0}]", string.Join(",", InteractRange))
                            ),
                            new RenderBlank()
                        },
                        {
                            new RenderText(AssetManager.WindowFont,
                                "Recovers HP: [" + HPHealed + "]"),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}
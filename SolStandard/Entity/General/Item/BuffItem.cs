using System.Collections.Generic;
using System.Linq;
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
    public class BuffItem : TerrainEntity, IActionTile, IConsumable
    {
        private static readonly int[] UseRange = {0, 1};

        public int[] InteractRange { get; private set; }
        public bool IsBroken { get; private set; }

        private readonly int statModifier;
        private readonly Stats statistic;
        private readonly int buffDuration;

        public BuffItem(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string statistic,
            int statModifier, int buffDuration, int[] pickupRange)
            : base(name, type, sprite, mapCoordinates, new Dictionary<string, string>())
        {
            InteractRange = pickupRange;
            this.statistic = UnitStatistics.Abbreviation.First(key => key.Value == statistic).Key;
            this.statModifier = statModifier;
            this.buffDuration = buffDuration;
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
            return new ConsumeBuffItemAction(this, statistic, statModifier, buffDuration, UseRange);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new BuffItem(Name, Type, Sprite, MapCoordinates, statistic.ToString().ToUpper(), statModifier,
                buffDuration, InteractRange);
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
                            new RenderText(AssetManager.WindowFont,
                                string.Format("[{0}{1}]", (statModifier > 0) ? "+" : "-", statModifier)
                            ),
                            UnitStatistics.GetSpriteAtlas(statistic)
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
                                string.Format("[{0}] Turns", buffDuration)
                            ),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}
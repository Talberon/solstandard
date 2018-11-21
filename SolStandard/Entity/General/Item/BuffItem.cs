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
            int statModifier, int buffDuration)
            : base(name, type, sprite, mapCoordinates, new Dictionary<string, string>())
        {
            InteractRange = new[] {0};
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

        public UnitAction TileAction()
        {
            return new PickUpItemAction(this, MapCoordinates);
        }

        public UnitAction UseAction()
        {
            return new ConsumeBuffItemAction(this, statistic, statModifier, buffDuration, UseRange);
        }

        public UnitAction DropAction()
        {
            return new DropItemAction(this);
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
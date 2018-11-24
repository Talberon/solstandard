using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Spoils : TerrainEntity, IActionTile
    {
        public int Gold { get; private set; }
        public List<IItem> Items { get; private set; }
        public int[] InteractRange { get; private set; }

        public Spoils(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int gold,
            List<IItem> items) :
            base(name, type, sprite, mapCoordinates, new Dictionary<string, string>())
        {
            Gold = gold;
            Items = items;
            InteractRange = new[] {0, 1};
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new TakeSpoilsAction(this)
            };
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
                            new SpriteAtlas(AssetManager.GoldIcon, new Vector2(GameDriver.CellSize)),
                            new RenderText(AssetManager.WindowFont, "Gold: " + Gold + Currency.CurrencyAbbreviation)
                        },
                        {
                            ItemDetails,
                            new RenderBlank()
                        }
                    },
                    1
                );
            }
        }

        private IRenderable ItemDetails
        {
            get
            {
                if (Items.Count <= 0) return new RenderBlank();

                IRenderable[,] content = new IRenderable[Items.Count, 2];

                for (int i = 0; i < Items.Count; i++)
                {
                    content[i, 0] = Items[i].Icon;
                    content[i, 1] = new RenderText(AssetManager.WindowFont, Items[i].Name);
                }

                return new WindowContentGrid(content, 2);
            }
        }
    }
}
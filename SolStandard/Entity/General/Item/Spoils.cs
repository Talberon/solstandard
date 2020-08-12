using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Spoils : TerrainEntity, IActionTile
    {
        public int Gold { get; }
        public List<IItem> Items { get; }
        public int[] InteractRange { get; }

        public Spoils(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int gold,
            List<IItem> items) :
            base(name, type, sprite, mapCoordinates)
        {
            Gold = gold;
            Items = items;
            InteractRange = new[] {0};
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new TakeSpoilsAction(this)
            };
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(new[,]
                {
                    {
                        MiscIconProvider.GetMiscIcon(MiscIcon.Gold, GameDriver.CellSizeVector),
                        new RenderText(AssetManager.WindowFont, "Gold: " + Gold + Currency.CurrencyAbbreviation)
                    },
                    {
                        ItemDetails,
                        RenderBlank.Blank
                    }
                },
                1,
                HorizontalAlignment.Centered
            );

        private IRenderable ItemDetails
        {
            get
            {
                if (Items.Count <= 0) return RenderBlank.Blank;

                var content = new IRenderable[Items.Count, 2];

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
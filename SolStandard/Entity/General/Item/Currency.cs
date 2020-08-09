using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Currency : TerrainEntity, IActionTile
    {
        public const string CurrencyAbbreviation = "G";

        public int Value { get; }
        public int[] InteractRange { get; }

        public Currency(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int value, int[] range) :
            base(name, type, sprite, mapCoordinates)
        {
            Value = value;
            InteractRange = range;
        }

        public static IRenderable GoldIcon(Vector2 size)
        {
            return MiscIconProvider.GetMiscIcon(MiscIcon.Gold, size);
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpCurrencyAction(this)
            };
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont, "Value: " + Value),
                        ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Taxes,
                            GameDriver.CellSizeVector)
                    }
                }
            );
    }
}
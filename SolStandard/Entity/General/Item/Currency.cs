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
    public class Currency : TerrainEntity, IActionTile
    {
        public const string CurrencyAbbreviation = "G";

        public int Value { get; private set; }
        public int[] InteractRange { get; private set; }

        public Currency(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int value, int[] range) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            Value = value;
            InteractRange = range;
        }

        public UnitAction TileAction()
        {
            return new PickUpCurrencyAction(this);
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
                            new RenderText(AssetManager.WindowFont, "Value: " + Value + CurrencyAbbreviation),
                            new RenderBlank()
                        }
                    },
                    1
                );
            }
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;
using SolStandard.Entity.Unit.Skills.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Currency : TerrainEntity, IActionTile
    {
        public const string CurrencyAbbreviation = "G";

        public int Value { get; private set; }
        public int[] Range { get; private set; }

        public Currency(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int value, int[] range) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            Value = value;
            Range = range;
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
                            Sprite,
                            new RenderText(AssetManager.HeaderFont, Name),
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "~~~~~~~~~"),
                            new RenderBlank()
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
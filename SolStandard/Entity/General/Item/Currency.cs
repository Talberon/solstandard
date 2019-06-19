using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Currency : TerrainEntity, IActionTile
    {
        public const string CurrencyAbbreviation = "G";

        public int Value { get; private set; }
        public int[] InteractRange { get; private set; }

        public Currency(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int value, int[] range) :
            base(name, type, sprite, mapCoordinates)
        {
            Value = value;
            InteractRange = range;
        }

        public static IRenderable GoldIcon(Vector2 size)
        {
            return new SpriteAtlas(AssetManager.GoldIcon, size);
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpCurrencyAction(this)
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
                            StatusIconProvider.GetStatusIcon(StatusIcon.PickupRange, new Vector2(GameDriver.CellSize)),
                            new RenderText(
                                AssetManager.WindowFont,
                                ": " + string.Format("[{0}]", string.Join(",", InteractRange))
                            )
                        },
                        {
                            new Window(new IRenderable[,]
                                {
                                    {
                                        new RenderText(AssetManager.WindowFont, "Value: " + Value),
                                        ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Taxes,
                                            new Vector2(GameDriver.CellSize))
                                    }
                                },
                                InnerWindowColor,
                                HorizontalAlignment.Centered
                            ),
                            new RenderBlank()
                        }
                    },
                    1
                );
            }
        }
    }
}
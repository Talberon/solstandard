using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class SeizeEntity : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; private set; }

        public readonly bool CapturableByBlue;
        public readonly bool CapturableByRed;

        public SeizeEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool capturableByBlue, bool capturableByRed) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            CapturableByBlue = capturableByBlue;
            CapturableByRed = capturableByRed;
            InteractRange = new[] {0};
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new SeizeAction(this, MapCoordinates)
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
                            new RenderText(
                                AssetManager.WindowFont,
                                (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor
                            )
                        },
                        {
                            (CapturableByBlue)
                                ? new Window(
                                    new RenderText(AssetManager.WindowFont, "Capturable by Blue"),
                                    TeamUtility.DetermineTeamColor(Team.Blue)
                                )
                                : new RenderBlank() as IRenderable,
                            new RenderBlank()
                        },
                        {
                            (CapturableByRed)
                                ? new Window(
                                    new RenderText(AssetManager.WindowFont, "Capturable by Red"),
                                    TeamUtility.DetermineTeamColor(Team.Red)
                                )
                                : new RenderBlank() as IRenderable,
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}
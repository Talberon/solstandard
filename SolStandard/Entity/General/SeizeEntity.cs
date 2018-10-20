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
    public class SeizeEntity : TerrainEntity, IActionTile
    {
        public int[] Range { get; private set; }

        public readonly bool CapturableByBlue;
        public readonly bool CapturableByRed;

        public SeizeEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool capturableByBlue, bool capturableByRed) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            CapturableByBlue = capturableByBlue;
            CapturableByRed = capturableByRed;
            Range = new[] {0};
        }

        public UnitAction TileAction()
        {
            return new SeizeAction(this, MapCoordinates);
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
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(
                                AssetManager.WindowFont,
                                (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor
                            )
                        },
                        {
                            new RenderText(
                                AssetManager.WindowFont,
                                (CapturableByBlue) ? "Capturable by Blue" : "",
                                (CapturableByBlue) ? TeamUtility.DetermineTeamColor(Team.Blue) : NegativeColor
                            ),
                            new RenderBlank()
                        },
                        {
                            new RenderText(
                                AssetManager.WindowFont,
                                (CapturableByRed) ? "Capturable by Red" : "",
                                (CapturableByRed) ? TeamUtility.DetermineTeamColor(Team.Red) : NegativeColor
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
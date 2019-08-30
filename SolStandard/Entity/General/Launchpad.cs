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
    public class Launchpad : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; }
        private readonly int[] launchRange;

        public Launchpad(string name, string type, Vector2 mapCoordinates, int[] launchRange) :
            base(name, type, SpringTrap.BuildSpringSprite(SpringType.Free), mapCoordinates)
        {
            CanMove = true;
            InteractRange = new[] {0};
            this.launchRange = launchRange;
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new LaunchpadAction(this, launchRange)
            };
        }

        public override IRenderable TerrainInfo =>
            new WindowContentGrid(
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
                        new Window(
                            new IRenderable[,]
                            {
                                {
                                    UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                                    new RenderText(AssetManager.WindowFont, "Range:"),
                                    new RenderText(AssetManager.WindowFont, $"[{string.Join(",", InteractRange)}]")
                                }
                            },
                            InnerWindowColor
                        ),
                        new RenderBlank()
                    }
                },
                1,
                HorizontalAlignment.Centered
            );
    }
}
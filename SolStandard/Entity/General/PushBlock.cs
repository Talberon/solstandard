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
    public class PushBlock : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; }

        public PushBlock(string name, string type, IRenderable sprite, Vector2 mapCoordinates) :
            base(name, type, sprite, mapCoordinates)
        {
            CanMove = false;
            InteractRange = new[] {1};
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PushBlockAction(this, MapCoordinates)
            };
        }

        public override IRenderable TerrainInfo =>
            new WindowContentGrid(
                new[,]
                {
                    {
                        base.TerrainInfo,
                        RenderBlank.Blank
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Mv),
                        new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                            (CanMove) ? PositiveColor : NegativeColor)
                    },
                },
                1,
                HorizontalAlignment.Centered
            );
    }
}
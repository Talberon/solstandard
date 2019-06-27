using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Movable : TerrainEntity
    {
        public Movable(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool canMove) :
            base(name, type, sprite, mapCoordinates)
        {
            CanMove = canMove;
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
                    }
                },
                3
            );
    }
}
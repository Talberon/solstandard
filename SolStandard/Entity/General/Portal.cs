using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Portal : TerrainEntity, IActionTile
    {
        private readonly bool canMove;
        private readonly string destinationId;
        public int[] InteractRange { get; }

        public Portal(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool canMove,
            string destinationId, int[] range) :
            base(name, type, sprite, mapCoordinates)
        {
            this.canMove = canMove;
            this.destinationId = destinationId;
            InteractRange = range;
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new Transport(this, destinationId)
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
                        new RenderText(AssetManager.WindowFont, (canMove) ? "Can Move" : "No Move",
                            (canMove) ? PositiveColor : NegativeColor)
                    },
                    {
                        new RenderText(AssetManager.WindowFont, "Destination: " + destinationId),
                        new RenderBlank()
                    },
                    {
                        new RenderText(AssetManager.WindowFont,
                            $"Range: [{string.Join(",", InteractRange)}]"),
                        new RenderBlank()
                    }
                },
                3
            );
    }
}
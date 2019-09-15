using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Portal : TerrainEntity, IActionTile
    {
        private readonly string destinationId;
        public int[] InteractRange { get; }

        public Portal(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string destinationId,
            int[] range) :
            base(name, type, sprite, mapCoordinates)
        {
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

        protected override IRenderable EntityInfo =>
            new RenderText(AssetManager.WindowFont, "Destination: " + destinationId);
    }
}
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Map;

namespace SolStandard.Utility.Events
{
    public class DropItemEvent : IEvent
    {
        private readonly TerrainEntity itemTile;
        private readonly Vector2 dropCoordinates;

        public DropItemEvent(TerrainEntity itemTile, Vector2 dropCoordinates)
        {
            this.dropCoordinates = dropCoordinates;
            this.itemTile = itemTile;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            if (GameContext.ActiveUnit.RemoveItemFromInventory(itemTile as IItem))
            {
                DropItemAtCoordinates();
                //TODO Play drop item SFX
            }

            Complete = true;
        }

        private void DropItemAtCoordinates()
        {
            itemTile.MapCoordinates = dropCoordinates;
            MapContainer.GameGrid[(int) Layer.Items][(int) dropCoordinates.X, (int) dropCoordinates.Y] = itemTile;
        }
    }
}
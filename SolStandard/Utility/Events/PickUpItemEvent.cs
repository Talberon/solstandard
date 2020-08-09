using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility.Events
{
    public class PickUpItemEvent : IEvent
    {
        private readonly IItem item;
        private readonly Vector2 itemCoordinates;
        public bool Complete { get; private set; }

        public PickUpItemEvent(IItem item, Vector2 itemCoordinates)
        {
            this.item = item;
            this.itemCoordinates = itemCoordinates;
        }

        public void Continue()
        {
            GlobalContext.ActiveUnit.AddItemToInventory(item);
            RemoveItemFromMap(item, itemCoordinates);
            AssetManager.MenuConfirmSFX.Play();
            WorldContext.WorldHUD.GenerateObjectiveWindow();

            AddItemToUnitInventoryEvent.ItemToast(GlobalContext.ActiveUnit, item);

            Complete = true;
        }

        private static void RemoveItemFromMap(IItem item, Vector2 coordinates)
        {
            MapContainer.GameGrid[
                (int) GetLayerForItem(item, MapContainer.GetMapSliceAtCoordinates(coordinates))
            ][(int) coordinates.X, (int) coordinates.Y] = null;
        }

        private static Layer GetLayerForItem(IItem item, MapSlice slice)
        {
            if (slice.ItemEntity == item) return Layer.Items;
            if (slice.TerrainEntity == item) return Layer.Entities;

            throw new ItemNotFoundException("No item available at coordinates: " + slice.MapCoordinates);
        }
    }
}
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Map;
using SolStandard.Utility.Assets;

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
            GameContext.ActiveUnit.AddItemToInventory(item);
            RemoveItemFromMap();
            AssetManager.MenuConfirmSFX.Play();
            
            AddItemToUnitInventoryEvent.ItemToast(GameContext.ActiveUnit, item);
            
            Complete = true;
        }

        private void RemoveItemFromMap()
        {
            MapContainer.GameGrid[(int) Layer.Items][(int) itemCoordinates.X, (int) itemCoordinates.Y] = null;
        }
    }
}
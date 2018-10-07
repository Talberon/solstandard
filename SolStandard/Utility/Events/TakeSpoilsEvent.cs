using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class TakeSpoilsEvent : IEvent
    {
        private readonly Spoils spoils;

        public TakeSpoilsEvent(Spoils spoils)
        {
            this.spoils = spoils;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.ActiveUnit.CurrentGold += spoils.Currency;

            foreach (IItem item in spoils.Items)
            {
                GameContext.ActiveUnit.AddItemToInventory(item);
            }

            RemoveItemFromMap();

            AssetManager.MenuConfirmSFX.Play();

            Complete = true;
        }


        private void RemoveItemFromMap()
        {
            MapContainer.GameGrid[(int) Layer.Items][(int) spoils.MapCoordinates.X, (int) spoils.MapCoordinates.Y] =
                null;
        }
    }
}
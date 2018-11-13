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
            GameContext.ActiveUnit.CurrentGold += spoils.Gold;

            foreach (IItem item in spoils.Items)
            {
                GameContext.ActiveUnit.AddItemToInventory(item);
            }

            RemoveItemFromMap();

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Retrieved spoils!", 50);
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
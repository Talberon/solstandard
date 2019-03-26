using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class DeleteItemEvent : IEvent
    {
        private readonly IItem itemToDelete;
        public bool Complete { get; private set; }

        public DeleteItemEvent(IItem itemToDelete)
        {
            this.itemToDelete = itemToDelete;
        }

        public void Continue()
        {
            GameContext.ActiveUnit.RemoveItemFromInventory(itemToDelete);
            AssetManager.MenuConfirmSFX.Play();
            Complete = true;
        }
    }
}
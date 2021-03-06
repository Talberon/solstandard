using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class TransferUnitItemEvent : IEvent
    {
        private readonly GameUnit givingUnit;
        private readonly GameUnit receivingUnit;
        private readonly IItem itemToGive;

        public TransferUnitItemEvent(GameUnit givingUnit, GameUnit receivingUnit, IItem itemToGive)
        {
            this.givingUnit = givingUnit;
            this.receivingUnit = receivingUnit;
            this.itemToGive = itemToGive;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            givingUnit.RemoveItemFromInventory(itemToGive);
            receivingUnit.AddItemToInventory(itemToGive);

            IRenderable toastContent = new WindowContentGrid(
                new[,]
                {
                    {
                        SpriteResizer.TryResizeRenderable(itemToGive.Icon, new Vector2(MapContainer.MapToastIconSize)),
                        new RenderText(
                            AssetManager.MapFont,
                            $"{givingUnit.Id} gave {itemToGive.Name} to {receivingUnit.Id}!"
                        )
                    }
                }
            );

            WorldContext.WorldHUD.GenerateObjectiveWindow();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(toastContent, 50);
            AssetManager.MenuConfirmSFX.Play();

            Complete = true;
        }
    }
}
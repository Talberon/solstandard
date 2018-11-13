using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class AddItemToUnitInventoryEvent : IEvent
    {
        private readonly GameUnit unit;
        private readonly IItem item;
        public bool Complete { get; private set; }

        public AddItemToUnitInventoryEvent(GameUnit unit, IItem item)
        {
            this.unit = unit;
            this.item = item;
        }

        public void Continue()
        {
            unit.AddItemToInventory(item);

            IRenderable itemToast = new WindowContentGrid(
                new[,]
                {
                    {
                        item.Icon,
                        new RenderText(AssetManager.MapFont, GameContext.ActiveUnit.Id + " got " + item.Name + "!"),
                    }
                },
                1
            );

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(itemToast, 50);
            AssetManager.SkillBuffSFX.Play();
            Complete = true;
        }
    }
}
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
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

            ItemToast(unit, item);
            AssetManager.SkillBuffSFX.Play();
            Complete = true;
        }

        public static void ItemToast(GameUnit unit, IItem item)
        {
            IRenderable itemToast = new WindowContentGrid(
                new[,]
                {
                    {
                        SpriteResizer.TryResizeRenderable(item.Icon, new Vector2(MapContainer.MapToastIconSize)),
                        new RenderText(AssetManager.MapFont, unit.Id + " got " + item.Name + "!")
                    }
                }
            );

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(itemToast, 50);
        }
    }
}
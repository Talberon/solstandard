using Microsoft.Xna.Framework;
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

            ItemToast(unit, item);
            AssetManager.SkillBuffSFX.Play();
            Complete = true;
        }

        public static void ItemToast(GameUnit unit, IItem item)
        {
            SpriteAtlas itemSpriteAtlas = item.Icon as SpriteAtlas;

            IRenderable toastIcon = (itemSpriteAtlas != null) ? itemSpriteAtlas.Resize(new Vector2(16)) : item.Icon;

            IRenderable itemToast = new WindowContentGrid(
                new[,]
                {
                    {
                        toastIcon,
                        new RenderText(AssetManager.MapFont, unit.Id + " got " + item.Name + "!"),
                    }
                },
                1
            );

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(itemToast, 50);
        }
    }
}
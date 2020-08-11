using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class GluttonRoutine : UnitAction, IRoutine
    {
        private const SkillIcon RoutineIcon = SkillIcon.Glutton;

        public GluttonRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, GameDriver.CellSizeVector),
                name: "Glutton Routine",
                description: "Consumes a random consumable item in inventory.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon => SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3));

        public bool CanBeReadied(CreepUnit creepUnit)
        {
            return HasConsumableItemInInventory(creepUnit);
        }

        public bool CanExecute
        {
            get
            {
                GameUnit consumer = GlobalContext.Units.Find(creep => creep.Actions.Contains(this));
                return HasConsumableItemInInventory(consumer);
            }
        }

        private static bool HasConsumableItemInInventory(GameUnit consumer)
        {
            return consumer.Inventory.Any(item => item is IConsumable);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (HasConsumableItemInInventory(GlobalContext.ActiveUnit))
            {
                GameUnit consumer = GlobalContext.ActiveUnit;
                List<IConsumable> consumables =
                    consumer.Inventory.Where(item => item is IConsumable).Cast<IConsumable>().ToList();
                consumables.Shuffle();
                IConsumable itemToConsume = consumables.First();

                GlobalEventQueue.QueueSingleEvent(new ToastAtCursorEvent("Consuming " + itemToConsume.Name + "!", 50));
                GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(50));
                itemToConsume.Consume(consumer);
                GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(50));
                GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCellCoordinates("No consumables in inventory!",
                    targetSlice.MapCoordinates, 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}
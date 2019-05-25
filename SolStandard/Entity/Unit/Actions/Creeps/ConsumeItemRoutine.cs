using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class ConsumeItemRoutine : UnitAction, IRoutine
    {
        //TODO Add unique icon
        private const SkillIcon RoutineIcon = SkillIcon.Cleanse;

        public ConsumeItemRoutine()
            : base(
                icon: SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2(GameDriver.CellSize)),
                name: "Consume Item Routine",
                description: "Consumes a random consumable item in inventory.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {0},
                freeAction: false
            )
        {
        }

        public IRenderable MapIcon
        {
            get { return SkillIconProvider.GetSkillIcon(RoutineIcon, new Vector2((float) GameDriver.CellSize / 3)); }
        }

        public virtual bool CanBeReadied(CreepUnit creepUnit)
        {
            return HasConsumableItemInInventory;
        }

        public bool CanExecute
        {
            get { return HasConsumableItemInInventory; }
        }

        private bool HasConsumableItemInInventory
        {
            get
            {
                GameUnit consumer = GameContext.Units.Find(creep => creep.Actions.Contains(this));
                return consumer.Inventory.Select(item => item is IConsumable).Any();
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (HasConsumableItemInInventory)
            {
                GameUnit consumer = GameContext.ActiveUnit;
                List<IItem> consumables = consumer.Inventory.Where(item => item is IConsumable).ToList();
                consumables.Shuffle();
                IItem itemToConsume = consumables.First();

                GlobalEventQueue.QueueSingleEvent(new ToastAtCursorEvent("Consuming " + itemToConsume.Name + "!", 50));
                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
                itemToConsume.UseAction().ExecuteAction(targetSlice);
                GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("No consumables in inventory!",
                    targetSlice.MapCoordinates, 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}
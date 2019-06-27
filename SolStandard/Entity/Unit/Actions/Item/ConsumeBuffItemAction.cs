using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class ConsumeBuffItemAction : UnitAction
    {
        private readonly IConsumable item;

        public ConsumeBuffItemAction(IConsumable item, Stats statistic, int statModifier, int buffDuration,
            int[] range) : base(
            icon: item.Icon.Clone(),
            name: "Consume",
            description:
            $"Single use. Target modifies {UnitStatistics.Abbreviation[statistic]} by [{statModifier}] for [{buffDuration}] turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: range,
            freeAction: true
        )
        {
            this.item = item;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit) || TargetIsSelfInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                item.Consume(targetUnit);

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }

            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a friendly unit in range!", 50);
            }
        }
    }
}
using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class ConsumeRecoveryItemAction : UnitAction
    {
        private readonly IConsumable item;
        private readonly int hpHealed;

        public ConsumeRecoveryItemAction(IConsumable item, int hpHealed, int[] range) : base(
            icon: item.Icon,
            name: "Recover HP: " + item.Name,
            description: "Single use. Target recovers [" + hpHealed + "] HP.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: range
        )
        {
            this.item = item;
            this.hpHealed = hpHealed;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit) || TargetIsSelfInRange(targetSlice, targetUnit))
            {
                item.Consume();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new RegenerateHealthEvent(targetUnit, hpHealed));
                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a friendly unit in range!", 50);
            }
        }
    }
}
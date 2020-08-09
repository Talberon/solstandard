using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class ConsumeRecoveryItemAction : UnitAction
    {
        private readonly HealthPotion potion;

        public ConsumeRecoveryItemAction(HealthPotion potion, int hpHealed, int[] range) : base(
            icon: potion.Icon.Clone(),
            name: "Recover <" + hpHealed + "> HP",
            description: "Single use. Target recovers [" + hpHealed + "] HP.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: range,
            freeAction: true
        )
        {
            this.potion = potion;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit) || TargetIsSelfInRange(targetSlice, targetUnit))
            {
                potion.Consume(targetUnit);

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a friendly unit in range!", 50);
            }
        }
    }
}
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
        private readonly HealthPotion potion;
        private readonly int hpHealed;

        public ConsumeRecoveryItemAction(HealthPotion potion, int hpHealed, int[] range) : base(
            icon: potion.Icon,
            name: "Recover HP: " + potion.Name,
            description: "Single use. Target recovers [" + hpHealed + "] HP.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: range,
            freeAction: true
        )
        {
            this.potion = potion;
            this.hpHealed = hpHealed;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit) || TargetIsSelfInRange(targetSlice, targetUnit))
            {
                potion.Consume(targetUnit);
                
                Queue<IEvent> eventQueue = new Queue<IEvent>();
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
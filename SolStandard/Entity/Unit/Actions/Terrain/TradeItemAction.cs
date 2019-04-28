using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class TradeItemAction : UnitAction
    {
        private readonly IItem item;

        public TradeItemAction(IItem item) : base(
            icon: item.Icon.Clone(),
            name: "Give: " + item.Name,
            description: "Give this item to an ally in range.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1},
            freeAction: false
        )
        {
            this.item = item;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit actingUnit = GameContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (CanGiveItemToAlly(targetUnit, actingUnit, targetSlice))
            {
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new TransferUnitItemEvent(actingUnit, targetUnit, item));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Cannot drop/give item here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static bool CanGiveItemToAlly(GameUnit targetUnit, GameUnit actingUnit, MapSlice targetSlice)
        {
            return targetUnit != null && targetUnit.Team == actingUnit.Team && targetUnit != actingUnit &&
                   targetSlice.DynamicEntity != null;
        }
    }
}
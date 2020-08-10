using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class DropGiveItemAction : UnitAction
    {
        private readonly IItem item;

        public DropGiveItemAction(IItem item) : base(
            icon: item.Icon.Clone(),
            name: "Drop/Give",
            description: "Drop this item on an empty item tile or give it to an ally.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1},
            freeAction: true
        )
        {
            this.item = item;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            var itemTile = item as TerrainEntity;
            GameUnit actingUnit = GlobalContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TradeItemAction.CanGiveItemToAlly(targetUnit, actingUnit, targetSlice))
            {
                new TradeItemAction(item).ExecuteAction(targetSlice);
            }
            else if (CanPlaceItemAtSlice(itemTile, targetSlice))
            {
                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new DropItemEvent(itemTile, targetSlice.MapCoordinates));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot drop/give item here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanPlaceItemAtSlice(TerrainEntity itemTile, MapSlice targetSlice)
        {
            return (
                       targetSlice.ItemEntity == null 
                       || targetSlice.ItemEntity is Spoils 
                       || targetSlice.ItemEntity is IItem
                       || targetSlice.ItemEntity is Currency
                       )
                   && itemTile != null
                   && targetSlice.DynamicEntity != null
                   && UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates);
        }
    }
}
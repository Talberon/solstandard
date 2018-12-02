using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class DropGiveItemAction : UnitAction
    {
        private readonly IItem item;

        public DropGiveItemAction(IItem item) : base(
            icon: item.Icon,
            name: "Drop/Give: " + item.Name,
            description: "Drop this item on an empty item tile or give it to an ally.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1}
        )
        {
            this.item = item;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            TerrainEntity itemTile = item as TerrainEntity;
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
            else if (CanPlaceItemAtSlice(itemTile, targetSlice))
            {
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new DropItemEvent(itemTile, targetSlice.MapCoordinates));
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

        private static bool CanGiveItemToAlly(GameUnit targetUnit, GameUnit actingUnit, MapSlice targetSlice)
        {
            return targetUnit != null && targetUnit.Team == actingUnit.Team && targetUnit != actingUnit &&
                   targetSlice.DynamicEntity != null;
        }

        private static bool CanPlaceItemAtSlice(TerrainEntity itemTile, MapSlice targetSlice)
        {
            return targetSlice.ItemEntity == null && itemTile != null && targetSlice.DynamicEntity != null;
        }
    }
}
using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Terrain
{
    public class DropItemAction : UnitAction
    {
        private readonly IItem item;

        public DropItemAction(IItem item) : base(
            icon: item.Icon,
            name: "Drop: " + item.Name,
            description: "Drop this item on an empty item tile.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0}
        )
        {
            this.item = item;
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            TerrainEntity itemTile = item as TerrainEntity;

            if (CanPlaceItemAtSlice(itemTile, targetSlice))
            {
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new DropItemEvent(itemTile, targetSlice.MapCoordinates));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanPlaceItemAtSlice(TerrainEntity itemTile, MapSlice targetSlice)
        {
            return targetSlice.ItemEntity == null && itemTile != null && targetSlice.DynamicEntity != null;
        }
    }
}
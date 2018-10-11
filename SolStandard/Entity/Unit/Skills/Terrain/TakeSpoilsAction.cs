using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Terrain
{
    public class TakeSpoilsAction : UnitAction
    {
        private readonly Spoils spoils;

        public TakeSpoilsAction(Spoils spoils) : base(
            icon: spoils.RenderSprite,
            name: "Claim Spoils",
            description: "Take all of the currency and items from the bag of spoils.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1}
        )
        {
            this.spoils = spoils;
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (SelectingItemAtUnitLocation(targetSlice))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new TakeSpoilsEvent(spoils));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        private bool SelectingItemAtUnitLocation(MapSlice targetSlice)
        {
            return spoils == targetSlice.ItemEntity &&
                   targetSlice.DynamicEntity != null;
        }
    }
}
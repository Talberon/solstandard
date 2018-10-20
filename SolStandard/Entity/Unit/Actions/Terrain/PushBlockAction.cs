using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class PushBlockAction : UnitAction
    {
        private readonly PushBlock pushBlock;
        private readonly Vector2 itemCoordinates;

        public PushBlockAction(PushBlock pushBlock, Vector2 itemCoordinates) : base(
            icon: pushBlock.RenderSprite,
            name: "Push",
            description: "Push the target a tile away from your unit's position.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null
        )
        {
            this.pushBlock = pushBlock;
            this.itemCoordinates = itemCoordinates;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) itemCoordinates.X, (int) itemCoordinates.Y] =
                new MapDistanceTile(TileSprite, itemCoordinates, 0, false);
            MapContainer.MapCursor.SnapCursorToCoordinates(itemCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext,
            BattleContext battleContext)
        {
            if (SelectingPushBlockInRange(targetSlice))
            {
                if (BlockNotObstructed)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new PushBlockEvent(pushBlock));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                MapContainer.AddNewToastAtMapCursor("Not a valid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool BlockNotObstructed
        {
            get
            {
                Vector2 oppositeCoordinates = Shove.DetermineShovePosition(
                    GameContext.ActiveUnit.UnitEntity.MapCoordinates, pushBlock.MapCoordinates);

                return UnitMovingContext.CanMoveAtCoordinates(oppositeCoordinates);
            }
        }

        private bool SelectingPushBlockInRange(MapSlice targetSlice)
        {
            return targetSlice.TerrainEntity == pushBlock &&
                   targetSlice.DynamicEntity != null;
        }
    }
}
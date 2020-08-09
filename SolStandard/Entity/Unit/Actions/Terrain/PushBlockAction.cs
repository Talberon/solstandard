﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext;
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
        private readonly Vector2 blockCoordinates;

        public PushBlockAction(PushBlock pushBlock, Vector2 blockCoordinates) : base(
            icon: pushBlock.RenderSprite,
            name: "Push",
            description: "Push the target a tile away from your unit's position.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: true
        )
        {
            this.pushBlock = pushBlock;
            this.blockCoordinates = blockCoordinates;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) blockCoordinates.X, (int) blockCoordinates.Y] =
                new MapDistanceTile(TileSprite, blockCoordinates);

            GameContext.GameMapContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(blockCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (SelectingPushBlockInRange(targetSlice))
            {
                if (BlockNotObstructed)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new PushBlockEvent(pushBlock));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a valid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private bool BlockNotObstructed
        {
            get
            {
                Vector2 oppositeCoordinates = DetermineOppositeTileOfUnit(
                    GameContext.ActiveUnit.UnitEntity.MapCoordinates, pushBlock.MapCoordinates);

                return UnitMovingContext.CanEndMoveAtCoordinates(oppositeCoordinates) &&
                       MapContainer.GetMapSliceAtCoordinates(oppositeCoordinates).TerrainEntity == null;
            }
        }

        private bool SelectingPushBlockInRange(MapSlice targetSlice)
        {
            return targetSlice.TerrainEntity == pushBlock &&
                   targetSlice.DynamicEntity != null;
        }
    }
}
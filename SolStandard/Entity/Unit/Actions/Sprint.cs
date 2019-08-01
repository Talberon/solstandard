using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.Unit.Actions
{
    public class Sprint : UnitAction
    {
        private readonly int maxDistance;

        public Sprint(int maxDistance) : base(
            icon: UnitStatistics.GetSpriteAtlas(Stats.Mv, GameDriver.CellSizeVector),
            name: "Sprint",
            description: "Move up to " + maxDistance + " additional spaces this turn." + Environment.NewLine +
                         "Can not move further than maximum " + UnitStatistics.Abbreviation[Stats.Mv] + ".",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: false
        )
        {
            this.maxDistance = maxDistance;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            int lowerMv = GameContext.ActiveUnit.Stats.Mv < maxDistance ? GameContext.ActiveUnit.Stats.Mv : maxDistance;

            UnitMovingContext unitMovingContext =
                new UnitMovingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement));
            unitMovingContext.GenerateMoveGrid(origin, lowerMv, GameContext.ActiveUnit.Team);

            //Delete the origin space to prevent players standing still and wasting action.
            MapContainer.GameGrid[(int) mapLayer][(int) origin.X, (int) origin.Y] = null;

            GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(origin);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanMove(GameContext.ActiveUnit))
            {
                if (CanMoveToTargetTile(targetSlice))
                {
                    const bool walkThroughAllies = true;

                    MapContainer.ClearDynamicAndPreviewGrids();

                    GameUnit actingUnit = GameContext.ActiveUnit;

                    List<Direction> directions = AStarAlgorithm.DirectionsToDestination(
                        actingUnit.UnitEntity.MapCoordinates, targetSlice.MapCoordinates, walkThroughAllies, true
                    );

                    Queue<IEvent> pathingEventQueue = new Queue<IEvent>();
                    foreach (Direction direction in directions)
                    {
                        if (direction == Direction.None) continue;

                        pathingEventQueue.Enqueue(new CreepMoveEvent(actingUnit, direction, walkThroughAllies));
                        pathingEventQueue.Enqueue(new WaitFramesEvent(5));
                    }

                    pathingEventQueue.Enqueue(new CreepMoveEvent(actingUnit, Direction.None));
                    pathingEventQueue.Enqueue(new MoveEntityToCoordinatesEvent(actingUnit.UnitEntity,
                        targetSlice.MapCoordinates));
                    pathingEventQueue.Enqueue(new CameraCursorPositionEvent(targetSlice.MapCoordinates));
                    pathingEventQueue.Enqueue(new EndTurnEvent());
                    GlobalEventQueue.QueueEvents(pathingEventQueue);
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a valid tile!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't move!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanMove(GameUnit unit)
        {
            return unit.Stats.Mv > 0;
        }
    }
}
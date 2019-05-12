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

namespace SolStandard.Entity.Unit.Actions.Duelist
{
    public class Shift : UnitAction
    {
        private readonly int maxDistance;

        public Shift(int maxDistance) : base(
            icon: UnitStatistics.GetSpriteAtlas(Stats.Mv, new Vector2(GameDriver.CellSize)),
            name: "Shift",
            description: "Move one additional space.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: null,
            freeAction: false
        )
        {
            this.maxDistance = maxDistance;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            UnitMovingContext unitMovingContext =
                new UnitMovingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement));
            unitMovingContext.GenerateMoveGrid(origin, maxDistance, GameContext.ActiveUnit.Team);

            GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(origin);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            //TODO Refactor this to be part of an "extra movement" skill and make Shift a child of that
            if (CanMoveToTargetTile(targetSlice))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                GameUnit actingUnit = GameContext.ActiveUnit;

                List<Direction> directions = AStarAlgorithm.DirectionsToDestination(
                    actingUnit.UnitEntity.MapCoordinates, targetSlice.MapCoordinates, false, true
                );

                Queue<IEvent> pathingEventQueue = new Queue<IEvent>();
                foreach (Direction direction in directions)
                {
                    if (direction == Direction.None) continue;

                    pathingEventQueue.Enqueue(new UnitMoveEvent(actingUnit, direction));
                    pathingEventQueue.Enqueue(new WaitFramesEvent(5));
                }

                pathingEventQueue.Enqueue(new UnitMoveEvent(actingUnit, Direction.None));
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
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
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
            icon: UnitStatistics.GetSpriteAtlas(Stats.Mv, new Vector2(GameDriver.CellSize)),
            name: "Sprint",
            description: "Move an extra " + maxDistance + " spaces at the expense of losing " +
                         maxDistance + " " + UnitStatistics.Abbreviation[Stats.Mv] + " for a turn.",
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
            if (CanMove(UnitSelector.SelectUnit(targetSlice.UnitEntity)))
            {
                if (CanMoveToTargetTile(targetSlice))
                {
                    const bool walkThroughAllies = true;

                    MapContainer.ClearDynamicAndPreviewGrids();

                    GameUnit actingUnit = GameContext.ActiveUnit;

                    List<Direction> directions = AStarAlgorithm.DirectionsToDestination(
                        actingUnit.UnitEntity.MapCoordinates, targetSlice.MapCoordinates, walkThroughAllies
                    );

                    Queue<IEvent> pathingEventQueue = new Queue<IEvent>();
                    foreach (Direction direction in directions)
                    {
                        if (direction == Direction.None) continue;

                        pathingEventQueue.Enqueue(new UnitMoveEvent(actingUnit, direction, walkThroughAllies));
                        pathingEventQueue.Enqueue(new WaitFramesEvent(5));
                    }

                    pathingEventQueue.Enqueue(new UnitMoveEvent(actingUnit, Direction.None));
                    pathingEventQueue.Enqueue(new MoveEntityToCoordinatesEvent(actingUnit.UnitEntity,
                        targetSlice.MapCoordinates));
                    pathingEventQueue.Enqueue(new CastStatusEffectEvent(actingUnit,
                        new ExhaustedStatus(2, -maxDistance)));
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
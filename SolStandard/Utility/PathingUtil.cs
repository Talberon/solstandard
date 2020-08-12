using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Utility
{
    public static class PathingUtil
    {
        public static Queue<IEvent> MoveToCoordinates(GameUnit movingUnit, Vector2 endCoordinates, bool ignoreLastStep,
            bool walkThroughAllies, int frameDelay)
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            
            var pathToItemQueue = new Queue<IEvent>();
            if (!movingUnit.IsAlive || movingUnit.UnitEntity == null) return pathToItemQueue;

            List<Direction> directionsToDestination = AStarAlgorithm.DirectionsToDestination(
                movingUnit.UnitEntity.MapCoordinates,
                endCoordinates,
                ignoreLastStep,
                walkThroughAllies,
                movingUnit.Team
            );

            foreach (Direction direction in directionsToDestination)
            {
                if (direction == Direction.None) continue;

                pathToItemQueue.Enqueue(new CreepMoveEvent(movingUnit, direction, walkThroughAllies));
                pathToItemQueue.Enqueue(new WaitFramesEvent(frameDelay));
            }

            pathToItemQueue.Enqueue(new CreepMoveEvent(movingUnit, Direction.None));
            
            MapContainer.ClearDynamicAndPreviewGrids();
            
            return pathToItemQueue;
        }
    }
}
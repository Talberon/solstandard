using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Priority_Queue;
using SolStandard.Containers.Components.World;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility
{
    public static class AStarAlgorithm
    {
        public static List<Direction> DirectionsToDestination(Vector2 origin, Vector2 destination,
            bool ignoreLastStep, bool walkThroughAllies, Team alliedTeam)
        {
            var frontier = new SimplePriorityQueue<MapDistanceTile>();

            frontier.Enqueue(
                new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), origin),
                0
            );

            var cameFrom = new Dictionary<MapDistanceTile, MapDistanceTile>();
            var costSoFar = new Dictionary<MapDistanceTile, int>();
            cameFrom[frontier.First] = null;
            costSoFar[frontier.First] = 0;

            while (frontier.Count > 0)
            {
                MapDistanceTile current = frontier.Dequeue();

                if (current.MapCoordinates == destination)
                {
                    return DeriveDirectionsFromPath(current, cameFrom, ignoreLastStep);
                }

                IEnumerable<MapDistanceTile> neighbours =
                    GetNeighbours(current, destination, walkThroughAllies, alliedTeam);

                foreach (MapDistanceTile neighbor in neighbours)
                {
                    int newCost = costSoFar[current] + 1;

                    if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = newCost;
                        int priority = newCost + DistanceFromGoal(destination, neighbor.MapCoordinates);
                        frontier.Enqueue(neighbor, priority);
                        cameFrom[neighbor] = current;
                    }
                }
            }

            throw new TileNotFoundException();
        }

        private static List<Direction> DeriveDirectionsFromPath(MapDistanceTile current,
            IReadOnlyDictionary<MapDistanceTile, MapDistanceTile> cameFrom, bool ignoreLastStep)
        {
            //Step backwards through the path and plot the directions from each of the nodes that map to the destination
            var path = new List<MapDistanceTile>();
            var directions = new List<Direction>();

            MapDistanceTile nextTile = current;

            if (!ignoreLastStep) path.Add(nextTile);

            while (cameFrom[nextTile] != null)
            {
                nextTile = cameFrom[nextTile];
                path.Add(nextTile);
            }

            foreach (MapDistanceTile tile in path)
            {
                if (cameFrom[tile] != null) directions.Insert(0, DetermineDirection(cameFrom[tile], tile));
            }

            return directions;
        }

        private static IEnumerable<MapDistanceTile> GetNeighbours(MapElement currentTile, Vector2 destination,
            bool walkThroughAllies, Team alliedTeam)
        {
            var neighbours = new List<MapDistanceTile>();

            var north = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y - 1);
            var south = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y + 1);
            var east = new Vector2(currentTile.MapCoordinates.X + 1, currentTile.MapCoordinates.Y);
            var west = new Vector2(currentTile.MapCoordinates.X - 1, currentTile.MapCoordinates.Y);

            if (
                WorldContext.CoordinatesWithinMapBounds(north) &&
                (UnitMovingPhase.CanEndMoveAtCoordinates(north) || north == destination ||
                 FriendlyUnitIsStandingHere(north, walkThroughAllies, alliedTeam))
            )
            {
                neighbours.Add(
                    new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), north)
                );
            }

            if (
                WorldContext.CoordinatesWithinMapBounds(south) &&
                (UnitMovingPhase.CanEndMoveAtCoordinates(south) || south == destination ||
                 FriendlyUnitIsStandingHere(south, walkThroughAllies, alliedTeam))
            )
            {
                neighbours.Add(
                    new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), south)
                );
            }

            if (
                WorldContext.CoordinatesWithinMapBounds(east) &&
                (UnitMovingPhase.CanEndMoveAtCoordinates(east) || east == destination ||
                 FriendlyUnitIsStandingHere(east, walkThroughAllies, alliedTeam))
            )
            {
                neighbours.Add(
                    new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), east)
                );
            }

            if (
                WorldContext.CoordinatesWithinMapBounds(west) &&
                (UnitMovingPhase.CanEndMoveAtCoordinates(west) || west == destination ||
                 FriendlyUnitIsStandingHere(west, walkThroughAllies, alliedTeam))
            )
            {
                neighbours.Add(
                    new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), west)
                );
            }

            return neighbours;
        }

        private static int DistanceFromGoal(Vector2 next, Vector2 current)
        {
            return Convert.ToInt32(Math.Abs(next.X - current.X) + Math.Abs(next.Y - current.Y));
        }

        private static bool FriendlyUnitIsStandingHere(Vector2 coordinates, bool walkThroughAllies, Team alliedTeam)
        {
            MapSlice slice = MapContainer.GetMapSliceAtCoordinates(coordinates);
            GameUnit unit = UnitSelector.SelectUnit(slice.UnitEntity);

            return unit != null && unit.Team == alliedTeam && walkThroughAllies;
        }

        private static Direction DetermineDirection(MapElement current, MapElement next)
        {
            var north = new Vector2(current.MapCoordinates.X, current.MapCoordinates.Y - 1);
            var south = new Vector2(current.MapCoordinates.X, current.MapCoordinates.Y + 1);
            var east = new Vector2(current.MapCoordinates.X + 1, current.MapCoordinates.Y);
            var west = new Vector2(current.MapCoordinates.X - 1, current.MapCoordinates.Y);

            if (next.MapCoordinates == north)
            {
                return Direction.Up;
            }

            if (next.MapCoordinates == south)
            {
                return Direction.Down;
            }

            if (next.MapCoordinates == east)
            {
                return Direction.Right;
            }

            if (next.MapCoordinates == west)
            {
                return Direction.Left;
            }

            throw new TileNotFoundException();
        }
    }
}
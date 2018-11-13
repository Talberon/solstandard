using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Priority_Queue;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Utility.Exceptions;

namespace SolStandard.Utility
{
    public class AStarAlgorithm
    {
        public static List<Direction> DirectionsToDestination(Vector2 origin, Vector2 destination)
        {
            SimplePriorityQueue<MapDistanceTile> frontier = new SimplePriorityQueue<MapDistanceTile>();

            frontier.Enqueue(
                new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), origin, 0),
                0
            );

            Dictionary<MapDistanceTile, MapDistanceTile> cameFrom = new Dictionary<MapDistanceTile, MapDistanceTile>();
            Dictionary<MapDistanceTile, int> costSoFar = new Dictionary<MapDistanceTile, int>();
            cameFrom[frontier.First] = null;
            costSoFar[frontier.First] = 0;

            while (frontier.Count > 0)
            {
                MapDistanceTile current = frontier.Dequeue();


                if (current.MapCoordinates == destination)
                {
                    //TODO step backwards through the path and plot the directions from each of the nodes that map to the destination
                    List<MapDistanceTile> path = new List<MapDistanceTile>();
                    List<Direction> directions = new List<Direction>();

                    MapDistanceTile nextTile = current;

                    while (cameFrom[nextTile] != null)
                    {
                        nextTile = cameFrom[nextTile];

                        path.Add(nextTile);
                    }

                    foreach (MapDistanceTile tile in path)
                    {
                        if (cameFrom[tile] != null) directions.Add(DetermineDirection(cameFrom[tile], tile));
                    }

                    return directions;
                }


                IEnumerable<MapDistanceTile> neighbours = GetNeighbours(current, destination);

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

        private static int DistanceFromGoal(Vector2 next, Vector2 current)
        {
            return Convert.ToInt32(Math.Abs(next.X - current.X) + Math.Abs(next.Y - current.Y));
        }

        private static IEnumerable<MapDistanceTile> GetNeighbours(MapElement currentTile, Vector2 destination)
        {
            List<MapDistanceTile> neighbours = new List<MapDistanceTile>();

            Vector2 north = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y - 1);
            Vector2 south = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y + 1);
            Vector2 east = new Vector2(currentTile.MapCoordinates.X + 1, currentTile.MapCoordinates.Y);
            Vector2 west = new Vector2(currentTile.MapCoordinates.X - 1, currentTile.MapCoordinates.Y);

            if (
                GameMapContext.CoordinatesWithinMapBounds(north) &&
                (UnitMovingContext.CanMoveAtCoordinates(north) || north == destination)
            )
            {
                neighbours.Add(
                    new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), north, 0)
                );
            }

            if (
                GameMapContext.CoordinatesWithinMapBounds(south) &&
                (UnitMovingContext.CanMoveAtCoordinates(south) || south == destination)
            )
            {
                neighbours.Add(
                    new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), south, 0)
                );
            }

            if (
                GameMapContext.CoordinatesWithinMapBounds(east) &&
                (UnitMovingContext.CanMoveAtCoordinates(east) || east == destination)
            )
            {
                neighbours.Add(
                    new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), east, 0)
                );
            }

            if (
                GameMapContext.CoordinatesWithinMapBounds(west) &&
                (UnitMovingContext.CanMoveAtCoordinates(west) || west == destination)
            )
            {
                neighbours.Add(
                    new MapDistanceTile(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement), west, 0)
                );
            }

            return neighbours;
        }

        private static Direction DetermineDirection(MapElement current, MapElement next)
        {
            Vector2 north = new Vector2(current.MapCoordinates.X, current.MapCoordinates.Y - 1);
            Vector2 south = new Vector2(current.MapCoordinates.X, current.MapCoordinates.Y + 1);
            Vector2 east = new Vector2(current.MapCoordinates.X + 1, current.MapCoordinates.Y);
            Vector2 west = new Vector2(current.MapCoordinates.X - 1, current.MapCoordinates.Y);

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
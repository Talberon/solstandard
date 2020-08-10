using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Containers.Components.World.SubContext.Movement
{
    public class UnitMovingPhase
    {
        private readonly SpriteAtlas spriteAtlas;

        public UnitMovingPhase(SpriteAtlas spriteAtlas)
        {
            this.spriteAtlas = spriteAtlas;
        }

        public void GenerateMoveGrid(Vector2 origin, int maxDistance, Team team, bool showNumbers = false)
        {
            //Breadth First Search Algorithm (with limit)
            var frontier = new Queue<MapDistanceTile>();

            var startTile = new MapDistanceTile(spriteAtlas, origin);
            frontier.Enqueue(startTile);

            List<MapDistanceTile> visited =
                DetermineMovableTiles(maxDistance, startTile, frontier, team, showNumbers);

            AddVisitedTilesToGameGrid(visited, Layer.Dynamic);
        }

        private List<MapDistanceTile> DetermineMovableTiles(int maximumDistance, MapDistanceTile startTile,
            Queue<MapDistanceTile> frontier, Team team, bool distanceVisible)
        {
            var visited = new List<MapDistanceTile> {startTile};

            while (frontier.Count > 0)
            {
                MapDistanceTile currentTile = frontier.Dequeue();

                if (currentTile.Distance >= maximumDistance) continue;

                IEnumerable<MapDistanceTile> neighbours =
                    GetNeighbours(currentTile, visited, team, distanceVisible);

                foreach (MapDistanceTile neighbour in neighbours)
                {
                    if (visited.Contains(neighbour)) continue;

                    frontier.Enqueue(neighbour);
                    visited.Add(neighbour);
                }
            }

            return visited;
        }

        private IEnumerable<MapDistanceTile> GetNeighbours(MapDistanceTile currentTile,
            List<MapDistanceTile> visitedTiles, Team team, bool distanceVisible)
        {
            var neighbours = new List<MapDistanceTile>();

            var north = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y - 1);
            var south = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y + 1);
            var east = new Vector2(currentTile.MapCoordinates.X + 1, currentTile.MapCoordinates.Y);
            var west = new Vector2(currentTile.MapCoordinates.X - 1, currentTile.MapCoordinates.Y);

            if (CanPlaceMoveTileAtCoordinates(north, visitedTiles, team))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.DrawSprite, north, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceMoveTileAtCoordinates(south, visitedTiles, team))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.DrawSprite, south, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceMoveTileAtCoordinates(east, visitedTiles, team))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.DrawSprite, east, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceMoveTileAtCoordinates(west, visitedTiles, team))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.DrawSprite, west, currentTile.Distance + 1, distanceVisible)
                );
            }

            return neighbours;
        }

        private static bool CanPlaceMoveTileAtCoordinates(Vector2 coordinates,
            IEnumerable<MapDistanceTile> visitedTiles, Team team)
        {
            if (!WorldContext.CoordinatesWithinMapBounds(coordinates)) return false;
            MapSlice slice = MapContainer.GetMapSliceAtCoordinates(coordinates);

            if (slice.UnitEntity != null)
            {
                GameUnit unitAtTile = UnitSelector.SelectUnit(slice.UnitEntity);
                if (unitAtTile.Team != team || unitAtTile.Team == Team.Creep)
                {
                    return false;
                }
            }

            if (visitedTiles.Any(tile => tile.MapCoordinates.Equals(coordinates))) return false;

            if (slice.TerrainEntity != null)
            {
                return slice.TerrainEntity.CanMove;
            }

            if (slice.CollideTile != null) return false;

            return true;
        }

        private static void AddVisitedTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles, Layer layer)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapContainer.GameGrid[(int) layer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] = tile;
            }
        }

        public static bool CanEndMoveAtCoordinates(Vector2 coordinates)
        {
            return CanEndMoveAtCoordinates(GlobalContext.ActiveUnit?.UnitEntity, coordinates);
        }

        public static bool CanEndMoveAtCoordinates(UnitEntity unitEntityEndingMove, Vector2 coordinates)
        {
            if (!WorldContext.CoordinatesWithinMapBounds(coordinates)) return false;

            MapSlice slice = MapContainer.GetMapSliceAtCoordinates(coordinates);

            if (slice.UnitEntity != null &&
                (GlobalContext.ActiveUnit == null || slice.UnitEntity != unitEntityEndingMove)) return false;

            if (slice.TerrainEntity != null)
            {
                return slice.TerrainEntity.CanMove;
            }

            if (slice.CollideTile != null) return false;

            return true;
        }
    }
}
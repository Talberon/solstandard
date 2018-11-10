using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts
{
    public class UnitMovingContext
    {
        private readonly SpriteAtlas spriteAtlas;

        public UnitMovingContext(SpriteAtlas spriteAtlas)
        {
            this.spriteAtlas = spriteAtlas;
        }

        public void GenerateMoveGrid(Vector2 origin, GameUnit unit, bool showNumbers = false)
        {
            //Breadth First Search Algorithm (with limit)
            Queue<MapDistanceTile> frontier = new Queue<MapDistanceTile>();

            MapDistanceTile startTile = new MapDistanceTile(spriteAtlas, origin, 0);
            frontier.Enqueue(startTile);

            List<MapDistanceTile> visited =
                DetermineMovableTiles(unit.Stats.Mv, startTile, frontier, unit.Team, showNumbers);

            AddVisitedTilesToGameGrid(visited, Layer.Dynamic);
        }

        private List<MapDistanceTile> DetermineMovableTiles(int maximumDistance, MapDistanceTile startTile,
            Queue<MapDistanceTile> frontier, Team team, bool distanceVisible)
        {
            List<MapDistanceTile> visited = new List<MapDistanceTile> {startTile};

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
            List<MapDistanceTile> neighbours = new List<MapDistanceTile>();

            Vector2 north = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y - 1);
            Vector2 south = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y + 1);
            Vector2 east = new Vector2(currentTile.MapCoordinates.X + 1, currentTile.MapCoordinates.Y);
            Vector2 west = new Vector2(currentTile.MapCoordinates.X - 1, currentTile.MapCoordinates.Y);

            if (CanPlaceMoveTileAtCoordinates(north, visitedTiles, team))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.SpriteAtlas, north, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceMoveTileAtCoordinates(south, visitedTiles, team))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.SpriteAtlas, south, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceMoveTileAtCoordinates(east, visitedTiles, team))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.SpriteAtlas, east, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceMoveTileAtCoordinates(west, visitedTiles, team))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.SpriteAtlas, west, currentTile.Distance + 1, distanceVisible)
                );
            }

            return neighbours;
        }

        private static bool CanPlaceMoveTileAtCoordinates(Vector2 coordinates,
            IEnumerable<MapDistanceTile> visitedTiles, Team team)
        {
            if (!GameMapContext.CoordinatesWithinMapBounds(coordinates)) return false;
            MapSlice slice = MapContainer.GetMapSliceAtCoordinates(coordinates);

            if (slice.UnitEntity != null && UnitSelector.SelectUnit(slice.UnitEntity).Team != team) return false;

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


        public static bool CanMoveAtCoordinates(Vector2 coordinates)
        {
            if (!GameMapContext.CoordinatesWithinMapBounds(coordinates)) return false;

            MapSlice slice = MapContainer.GetMapSliceAtCoordinates(coordinates);
            if (slice.UnitEntity != null && slice.UnitEntity != GameContext.ActiveUnit.UnitEntity) return false;

            if (slice.TerrainEntity != null)
            {
                return slice.TerrainEntity.CanMove;
            }

            if (slice.CollideTile != null) return false;

            return true;
        }
    }
}
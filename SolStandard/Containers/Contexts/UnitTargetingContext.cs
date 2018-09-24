using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;

namespace SolStandard.Containers.Contexts
{
    public class UnitTargetingContext
    {
        private readonly SpriteAtlas spriteAtlas;

        public UnitTargetingContext(SpriteAtlas spriteAtlas)
        {
            this.spriteAtlas = spriteAtlas;
        }

        public void GenerateTargetingGrid(Vector2 origin, int[] ranges)
        {
            //Breadth First Search Algorithm (with limit)
            Queue<MapDistanceTile> frontier = new Queue<MapDistanceTile>();

            MapDistanceTile startTile = new MapDistanceTile(spriteAtlas, origin, 0);
            frontier.Enqueue(startTile);

            List<MapDistanceTile> visited = new List<MapDistanceTile> {startTile};

            while (frontier.Count > 0)
            {
                MapDistanceTile currentTile = frontier.Dequeue();

                if (currentTile.Distance >= ranges.Max()) continue;

                IEnumerable<MapDistanceTile> neighbours = GetNeighbours(currentTile, visited);

                foreach (MapDistanceTile neighbour in neighbours)
                {
                    if (visited.Contains(neighbour)) continue;

                    frontier.Enqueue(neighbour);
                    visited.Add(neighbour);
                }
            }

            visited = RemoveTilesOutOfRange(visited, ranges);

            AddVisitedTilesToGameGrid(visited);
        }

        private static List<MapDistanceTile> RemoveTilesOutOfRange(List<MapDistanceTile> visited, int[] ranges)
        {
            List<MapDistanceTile> tilesToKeep = new List<MapDistanceTile>();

            foreach (int range in ranges)
            {
                foreach (MapDistanceTile tile in visited)
                {
                    if (tile.Distance == range)
                    {
                        tilesToKeep.Add(tile);
                    }
                }
            }

            return tilesToKeep;
        }

        private IEnumerable<MapDistanceTile> GetNeighbours(MapDistanceTile currentTile,
            List<MapDistanceTile> visitedTiles)
        {
            List<MapDistanceTile> neighbours = new List<MapDistanceTile>();

            Vector2 north = new Vector2(currentTile.Coordinates.X, currentTile.Coordinates.Y - 1);
            Vector2 south = new Vector2(currentTile.Coordinates.X, currentTile.Coordinates.Y + 1);
            Vector2 east = new Vector2(currentTile.Coordinates.X + 1, currentTile.Coordinates.Y);
            Vector2 west = new Vector2(currentTile.Coordinates.X - 1, currentTile.Coordinates.Y);

            if (CanMoveAtCoordinates(north, visitedTiles))
            {
                neighbours.Add(new MapDistanceTile(currentTile.SpriteAtlas, north, currentTile.Distance + 1));
            }

            if (CanMoveAtCoordinates(south, visitedTiles))
            {
                neighbours.Add(new MapDistanceTile(currentTile.SpriteAtlas, south, currentTile.Distance + 1));
            }

            if (CanMoveAtCoordinates(east, visitedTiles))
            {
                neighbours.Add(new MapDistanceTile(currentTile.SpriteAtlas, east, currentTile.Distance + 1));
            }

            if (CanMoveAtCoordinates(west, visitedTiles))
            {
                neighbours.Add(new MapDistanceTile(currentTile.SpriteAtlas, west, currentTile.Distance + 1));
            }

            return neighbours;
        }

        private static bool CanMoveAtCoordinates(Vector2 coordinates, IEnumerable<MapDistanceTile> visitedTiles)
        {
            if (visitedTiles.Any(tile => tile.Coordinates == coordinates)) return false;

            return MapContext.CoordinatesWithinMapBounds(coordinates);
        }

        private void AddVisitedTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapContainer.GameGrid[(int) Layer.Dynamic][(int) tile.Coordinates.X, (int) tile.Coordinates.Y] = tile;
            }
        }


        public static bool CanMoveAtCoordinates(Vector2 coordinates)
        {
            if (!MapContext.CoordinatesWithinMapBounds(coordinates)) return false;

            MapSlice slice = MapContainer.GetMapSliceAtCoordinates(coordinates);
            if (slice.UnitEntity != null) return false;

            if (slice.TerrainEntity != null && slice.TerrainEntity.Type != "Decoration")
            {
                if (slice.TerrainEntity.TiledProperties["canMove"] == "true") return true;
                if (slice.TerrainEntity.TiledProperties["canMove"] == "false") return false;
            }

            if (slice.CollideTile != null) return false;

            return true;
        }
    }
}
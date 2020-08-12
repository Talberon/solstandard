using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Containers.Components.World.SubContext.Targeting
{
    public class UnitTargetingPhase
    {
        private readonly IRenderable spriteAtlas;
        private readonly bool numbersVisible;

        public UnitTargetingPhase(IRenderable spriteAtlas, bool numbersVisible = false)
        {
            this.spriteAtlas = spriteAtlas;
            this.numbersVisible = numbersVisible;
        }

        public void GenerateTargetingGrid(Vector2 origin, int[] ranges, Layer mapLayer = Layer.Dynamic)
        {
            List<MapDistanceTile> visited = GetTargetingTiles(origin, ranges, numbersVisible);

            AddVisitedTilesToGameGrid(visited, mapLayer);
        }

        public void GenerateThreatGrid(Vector2 origin, IThreatRange threatRange, Team team = Team.Creep)
        {
            new UnitMovingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Dark))
                .GenerateMoveGrid(origin, threatRange.MvRange, team);

            foreach (MapElement mapElement in MapContainer.GameGrid[(int) Layer.Dynamic])
            {
                var tile = (MapDistanceTile) mapElement;
                if (tile == null) continue;

                //Generate attack tiles for the perimeter of the grid
                GenerateTargetingGrid(tile.MapCoordinates, threatRange.AtkRange, Layer.Preview);
            }

            //Clean up overlapping tiles
            foreach (MapElement mapElement in MapContainer.GameGrid[(int) Layer.Dynamic])
            {
                var tile = (MapDistanceTile) mapElement;
                if (tile != null)
                {
                    MapContainer.GameGrid[(int) Layer.Preview][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y]
                        = null;
                }
            }
        }

        public List<MapDistanceTile> GetTargetingTiles(Vector2 origin, int[] ranges, bool distanceVisible = false)
        {
            //Breadth First Search Algorithm (with limit)
            var frontier = new Queue<MapDistanceTile>();

            var startTile = new MapDistanceTile(spriteAtlas, origin, 0, distanceVisible);
            frontier.Enqueue(startTile);

            var visited = new List<MapDistanceTile> {startTile};

            while (frontier.Count > 0)
            {
                MapDistanceTile currentTile = frontier.Dequeue();

                if (currentTile.Distance >= ranges.Max()) continue;

                List<Vector2> visitedCoordinates = visited.Select(tile => tile.MapCoordinates).ToList();
                IEnumerable<MapDistanceTile> neighbours =
                    GetNeighbours(currentTile, visitedCoordinates, distanceVisible);

                foreach (MapDistanceTile neighbour in neighbours)
                {
                    if (visited.Contains(neighbour)) continue;

                    frontier.Enqueue(neighbour);
                    visited.Add(neighbour);
                }
            }

            visited = RemoveTilesOutOfRange(visited, ranges);
            return visited;
        }

        private static List<MapDistanceTile> RemoveTilesOutOfRange(IReadOnlyCollection<MapDistanceTile> visited,
            IEnumerable<int> ranges)
        {
            return (from range in ranges from tile in visited where tile.Distance == range select tile).ToList();
        }

        private static IEnumerable<MapDistanceTile> GetNeighbours(MapDistanceTile currentTile,
            IReadOnlyCollection<Vector2> visitedCoordinates, bool distanceVisible)
        {
            var neighbours = new List<MapDistanceTile>();

            var north = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y - 1);
            var south = new Vector2(currentTile.MapCoordinates.X, currentTile.MapCoordinates.Y + 1);
            var east = new Vector2(currentTile.MapCoordinates.X + 1, currentTile.MapCoordinates.Y);
            var west = new Vector2(currentTile.MapCoordinates.X - 1, currentTile.MapCoordinates.Y);


            if (CanPlaceTileAtCoordinates(north, visitedCoordinates))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.DrawSprite, north, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceTileAtCoordinates(south, visitedCoordinates))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.DrawSprite, south, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceTileAtCoordinates(east, visitedCoordinates))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.DrawSprite, east, currentTile.Distance + 1, distanceVisible)
                );
            }

            if (CanPlaceTileAtCoordinates(west, visitedCoordinates))
            {
                neighbours.Add(
                    new MapDistanceTile(currentTile.DrawSprite, west, currentTile.Distance + 1, distanceVisible)
                );
            }

            return neighbours;
        }

        private static bool CanPlaceTileAtCoordinates(Vector2 coordinates, IEnumerable<Vector2> unavailableCoordinates)
        {
            if (unavailableCoordinates.Contains(coordinates)) return false;

            return WorldContext.CoordinatesWithinMapBounds(coordinates);
        }

        private static void AddVisitedTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles, Layer layer)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapContainer.GameGrid[(int) layer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] = tile;
            }
        }
    }
}
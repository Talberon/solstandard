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

        public void GenerateMoveGrid(Vector2 origin, int maximumDistance, GameUnit selectedUnit)
        {
            //Breadth First Search Algorithm (with limit)
            Queue<MapDistanceTile> frontier = new Queue<MapDistanceTile>();

            MapDistanceTile startTile = new MapDistanceTile(spriteAtlas, origin, 0);
            frontier.Enqueue(startTile);

            List<MapDistanceTile> visited = new List<MapDistanceTile> {startTile};

            while (frontier.Count > 0)
            {
                MapDistanceTile currentTile = frontier.Dequeue();

                if (currentTile.Distance >= maximumDistance) continue;

                IEnumerable<MapDistanceTile> neighbours = GetNeighbours(currentTile, visited, selectedUnit);

                foreach (MapDistanceTile neighbour in neighbours)
                {
                    if (visited.Contains(neighbour)) continue;

                    frontier.Enqueue(neighbour);
                    visited.Add(neighbour);
                }
            }

            AddVisitedTilesToGameGrid(visited);
        }

        private IEnumerable<MapDistanceTile> GetNeighbours(MapDistanceTile currentTile, List<MapDistanceTile> visitedTiles,
            GameUnit selectedUnit)
        {
            List<MapDistanceTile> neighbours = new List<MapDistanceTile>();

            Vector2 north = new Vector2(currentTile.Coordinates.X, currentTile.Coordinates.Y - 1);
            Vector2 south = new Vector2(currentTile.Coordinates.X, currentTile.Coordinates.Y + 1);
            Vector2 east = new Vector2(currentTile.Coordinates.X + 1, currentTile.Coordinates.Y);
            Vector2 west = new Vector2(currentTile.Coordinates.X - 1, currentTile.Coordinates.Y);

            if (CanMoveAtCoordinates(north, visitedTiles, selectedUnit))
            {
                neighbours.Add(new MapDistanceTile(currentTile.SpriteAtlas, north, currentTile.Distance + 1));
            }

            if (CanMoveAtCoordinates(south, visitedTiles, selectedUnit))
            {
                neighbours.Add(new MapDistanceTile(currentTile.SpriteAtlas, south, currentTile.Distance + 1));
            }

            if (CanMoveAtCoordinates(east, visitedTiles, selectedUnit))
            {
                neighbours.Add(new MapDistanceTile(currentTile.SpriteAtlas, east, currentTile.Distance + 1));
            }

            if (CanMoveAtCoordinates(west, visitedTiles, selectedUnit))
            {
                neighbours.Add(new MapDistanceTile(currentTile.SpriteAtlas, west, currentTile.Distance + 1));
            }

            return neighbours;
        }

        private bool CanMoveAtCoordinates(Vector2 coordinates, IEnumerable<MapDistanceTile> visitedTiles,
            GameUnit selectedUnit)
        {
            MapSlice slice = MapContainer.GetMapSliceAtCoordinates(coordinates);

            if (slice.UnitEntity != null && slice.UnitEntity.TiledProperties["Team"] != selectedUnit.UnitTeam.ToString()) return false;
            
            if (visitedTiles.Any(tile => tile.Coordinates.Equals(coordinates))) return false;

            if (slice.GeneralEntity != null && slice.GeneralEntity.Type != "Decoration")
            {
                if (slice.GeneralEntity.TiledProperties["canMove"] == "true") return true;
                if (slice.GeneralEntity.TiledProperties["canMove"] == "false") return false;
            }

            if (slice.CollideTile != null) return false;

            return true;
        }

        private void AddVisitedTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapContainer.GameGrid[(int) Layer.Dynamic][(int) tile.Coordinates.X, (int) tile.Coordinates.Y] = tile;
            }
        }
    }
}
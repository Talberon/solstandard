using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Logic;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.Contexts
{
    public class MapContext
    {
        public enum TurnState
        {
            SelectUnit,
            UnitMoving,
            UnitDecidingAction,
            UnitTargetting,
            UnitActing,
            UnitFinishedActing,
            ResolvingTurn
        }

        public TurnState CurrentTurnState { get; private set; }
        public GameUnit SelectedUnit { get; set; }
        private readonly MapLayer mapLayer;

        public MapContext(MapLayer mapLayer)
        {
            this.mapLayer = mapLayer;
            CurrentTurnState = TurnState.SelectUnit;
        }

        public void ProceedToNextState()
        {
            if (CurrentTurnState == TurnState.ResolvingTurn)
            {
                CurrentTurnState = 0;
                Trace.WriteLine("Resetting to initial state: " + CurrentTurnState);
            }
            else
            {
                CurrentTurnState++;
                Trace.WriteLine("Changing state: " + CurrentTurnState);
            }
        }

        public MapLayer MapLayer
        {
            get { return mapLayer; }
        }


        private List<MapDistanceTile> GetNeighbours(MapDistanceTile currentTile, List<MapDistanceTile> visitedTiles)
        {
            List<MapDistanceTile> neighbours = new List<MapDistanceTile>();

            Vector2 north = new Vector2(currentTile.Coordinates.X, currentTile.Coordinates.Y - 1);
            Vector2 south = new Vector2(currentTile.Coordinates.X, currentTile.Coordinates.Y + 1);
            Vector2 east = new Vector2(currentTile.Coordinates.X + 1, currentTile.Coordinates.Y);
            Vector2 west = new Vector2(currentTile.Coordinates.X - 1, currentTile.Coordinates.Y);

            if (CanMoveAtCoordinates(north, visitedTiles))
            {
                neighbours.Add(new MapDistanceTile(currentTile.TextureCell, north, currentTile.Distance + 1, currentTile.Font));
            }

            if (CanMoveAtCoordinates(south, visitedTiles))
            {
                neighbours.Add(new MapDistanceTile(currentTile.TextureCell, south, currentTile.Distance + 1, currentTile.Font));
            }

            if (CanMoveAtCoordinates(east, visitedTiles))
            {
                neighbours.Add(new MapDistanceTile(currentTile.TextureCell, east, currentTile.Distance + 1, currentTile.Font));
            }

            if (CanMoveAtCoordinates(west, visitedTiles))
            {
                neighbours.Add(new MapDistanceTile(currentTile.TextureCell, west, currentTile.Distance + 1, currentTile.Font));
            }

            return neighbours;
        }

        //TODO the units list should be fetchable by the context; maybe it doesn't belong here or Units List should be somewhere else
        private bool CanMoveAtCoordinates(Vector2 coordinates, IEnumerable<MapDistanceTile> visitedTiles)
        {
            MapSlice slice = MapLayer.GetMapSliceAtCoordinates(coordinates);

            if (slice.CollideTile != null) return false;
            if (slice.GeneralEntity != null && slice.GeneralEntity.TiledProperties["canMove"] == "false") return false;
            if (slice.UnitEntity != null && slice.UnitEntity.TiledProperties["Team"] != SelectedUnit.UnitTeam.ToString()
            ) return false; //TODO figure out a better way to check this that's type-safe
            //TODO Make sure we can't create a move tile where one already exists
            foreach (MapDistanceTile tile in visitedTiles)
            {
                if (tile.Coordinates == coordinates) return false;
            }

            return true;
        }

        public void GenerateMoveGrid(Vector2 origin, int maximumDistance, TextureCell textureCell, ISpriteFont font)
        {
            //Breadth First Search Algorithm (with limit)
            Stack<MapDistanceTile> frontier = new Stack<MapDistanceTile>();

            MapDistanceTile startTile = new MapDistanceTile(textureCell, origin, 0, font);
            frontier.Push(startTile);

            List<MapDistanceTile> visited = new List<MapDistanceTile> {startTile};

            while (frontier.Count > 0)
            {
                MapDistanceTile currentTile = frontier.Pop();

                if (currentTile.Distance >= maximumDistance) continue;

                foreach (MapDistanceTile neighbor in GetNeighbours(currentTile, visited))
                {
                    if (visited.Contains(neighbor)) continue;

                    frontier.Push(neighbor);
                    visited.Add(neighbor);
                }
            }

            AddVisitedTilesToGameGrid(visited);
        }

        private void AddVisitedTilesToGameGrid(IEnumerable<MapDistanceTile> visitedTiles)
        {
            foreach (MapDistanceTile tile in visitedTiles)
            {
                MapLayer.GameGrid[(int) Layer.Dynamic][(int) tile.Coordinates.X, (int) tile.Coordinates.Y] = tile;
            }
        }
    }
}
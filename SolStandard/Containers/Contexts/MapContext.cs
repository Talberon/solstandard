using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Utility;

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
        private Vector2 selectedUnitOriginalPosition;
        private readonly MapLayer mapLayer;

        public MapContext(MapLayer mapLayer)
        {
            this.mapLayer = mapLayer;
            CurrentTurnState = TurnState.SelectUnit;
            selectedUnitOriginalPosition = new Vector2();
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

        public void GenerateMoveGrid(Vector2 origin, int maximumDistance, TextureCell textureCell)
        {
            selectedUnitOriginalPosition = origin;
            UnitMovingContext unitMovingContext = new UnitMovingContext(mapLayer, textureCell);
            unitMovingContext.GenerateMoveGrid(origin, maximumDistance, SelectedUnit);
        }

        public void MoveCursorAndSelectedUnitWithinMoveGrid(Direction direction)
        {
            if (TargetTileHasADynamicTile(direction))
            {
                SelectedUnit.MoveUnitInDirection(direction, mapLayer.MapGridSize);
                mapLayer.MapCursor.MoveCursorInDirection(direction);
            }
        }

        private bool TargetTileHasADynamicTile(Direction direction)
        {
            Vector2 targetPosition = MapLayer.MapCursor.MapCoordinates;
            
            switch (direction)
            {
                case Direction.Down:
                    targetPosition = new Vector2(targetPosition.X, targetPosition.Y + 1);
                    break;
                case Direction.Right:
                    targetPosition = new Vector2(targetPosition.X + 1, targetPosition.Y);
                    break;
                case Direction.Up:
                    targetPosition = new Vector2(targetPosition.X, targetPosition.Y - 1);
                    break;
                case Direction.Left:
                    targetPosition = new Vector2(targetPosition.X - 1, targetPosition.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            return MapLayer.GetMapSliceAtCoordinates(targetPosition).DynamicEntity != null;
        }
        
        public bool OtherUnitExistsAtCursor()
        {
            return OtherUnitExistsAtCoordinates(mapLayer.MapCursor.MapCoordinates);
        }
        
        public bool OtherUnitExistsAtCoordinates(Vector2 coordinates)
        {
            if (MapLayer.GameGrid[(int) Layer.Units][(int) coordinates.X, (int) coordinates.Y] == SelectedUnit.MapEntity)
            {
                return false;
            }
            
            return MapLayer.GameGrid[(int) Layer.Units][(int) coordinates.X, (int) coordinates.Y] != null;
        }

        public void MoveUnitOnMapGrid()
        {
            MapLayer.GameGrid[(int) Layer.Units][(int) selectedUnitOriginalPosition.X,
                (int) selectedUnitOriginalPosition.Y] = null;
            MapLayer.GameGrid[(int) Layer.Units][(int) SelectedUnit.MapEntity.MapCoordinates.X,
                (int) SelectedUnit.MapEntity.MapCoordinates.Y] = SelectedUnit.MapEntity;
        }

        public void ReturnUnitToOriginalPosition()
        {
            SelectedUnit.MapEntity.MapCoordinates = selectedUnitOriginalPosition;
            mapLayer.MapCursor.MapCoordinates = selectedUnitOriginalPosition;
        }
        
    }
}
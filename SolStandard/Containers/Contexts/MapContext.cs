using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Logic;
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
            UnitTargeting,
            UnitActing,
            UnitFinishedActing,
            ResolvingTurn
        }

        public TurnState CurrentTurnState { get; private set; }
        public GameUnit SelectedUnit { get; set; }
        private Vector2 selectedUnitOriginalPosition;
        private readonly MapContainer mapContainer;

        public MapContext(MapContainer mapContainer)
        {
            this.mapContainer = mapContainer;
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

        public MapContainer MapContainer
        {
            get { return mapContainer; }
        }

        public void GenerateMoveGrid(Vector2 origin, int maximumDistance, TextureCell textureCell)
        {
            selectedUnitOriginalPosition = origin;
            UnitMovingContext unitMovingContext = new UnitMovingContext(mapContainer, textureCell);
            unitMovingContext.GenerateMoveGrid(origin, maximumDistance, SelectedUnit);
        }

        public void MoveCursorAndSelectedUnitWithinMoveGrid(Direction direction)
        {
            if (TargetTileHasADynamicTile(direction))
            {
                SelectedUnit.MoveUnitInDirection(direction, mapContainer.MapGridSize);
                mapContainer.MapCursor.MoveCursorInDirection(direction);
            }
        }

        private bool TargetTileHasADynamicTile(Direction direction)
        {
            Vector2 targetPosition = MapContainer.MapCursor.MapCoordinates;

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

            return MapContainer.GetMapSliceAtCoordinates(targetPosition).DynamicEntity != null;
        }


        public bool OtherUnitExistsAtCursor()
        {
            return OtherUnitExistsAtCoordinates(mapContainer.MapCursor.MapCoordinates);
        }

        public bool OtherUnitExistsAtCoordinates(Vector2 coordinates)
        {
            
            if (UnitSelector.FindUnitEntityAtCoordinates(coordinates) == SelectedUnit.MapEntity)
            {
                return false;
            }

            return UnitSelector.FindUnitEntityAtCoordinates(coordinates) != null;
        }

        public void ReturnUnitToOriginalPosition()
        {
            SelectedUnit.MapEntity.MapCoordinates = selectedUnitOriginalPosition;
            mapContainer.MapCursor.MapCoordinates = selectedUnitOriginalPosition;
        }

        public void GenerateTargetingGridAtUnit(TextureCell textureCell)
        {
            selectedUnitOriginalPosition = SelectedUnit.MapEntity.MapCoordinates;
            GenerateTargetingGridAtCoordinates(selectedUnitOriginalPosition, SelectedUnit.Stats.AtkRange, textureCell);
        }

        public void GenerateTargetingGridAtCoordinates(Vector2 origin, int[] range, TextureCell textureCell)
        {
            selectedUnitOriginalPosition = origin;
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(mapContainer, textureCell);
            unitTargetingContext.GenerateTargetingGrid(origin, range);
        }
    }
}
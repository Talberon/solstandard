﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
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
            ResolvingTurn
        }

        public TurnState CurrentTurnState { get; private set; }
        public GameUnit SelectedUnit { get; set; }
        private Vector2 selectedUnitOriginalPosition;
        private readonly MapContainer mapContainer;

        public MapUI MapUI { get; private set; }

        public MapContext(MapContainer mapContainer, MapUI mapUI)
        {
            this.mapContainer = mapContainer;
            MapUI = mapUI;
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
        
        public void SetPromptWindowText(string promptText)
        {
            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(GameDriver.WindowFont, promptText),
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank()
                },
                {
                    new RenderText(GameDriver.WindowFont, "["),
                    new RenderText(GameDriver.WindowFont, "Press "),
                    new RenderText(GameDriver.WindowFont, "(A)", Color.Green),
                    new RenderText(GameDriver.WindowFont, "]")
                }
            };
            WindowContentGrid promptWindowContentGrid = new WindowContentGrid(promptTextContent, 2);
            MapUI.GenerateUserPromptWindow(promptWindowContentGrid, new Vector2(300, 100));
        }

        public void ConfirmPromptWindow()
        {
            MapUI.UserPromptWindow.Visible = false;
        }

        public void GenerateMoveGrid(Vector2 origin, int maximumDistance, SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = origin;
            UnitMovingContext unitMovingContext = new UnitMovingContext(mapContainer, spriteAtlas);
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
            return UnitSelector.FindOtherUnitEntityAtCoordinates(coordinates, SelectedUnit.MapEntity) != null;
        }

        public void ReturnUnitToOriginalPosition()
        {
            SelectedUnit.MapEntity.MapCoordinates = selectedUnitOriginalPosition;
            mapContainer.MapCursor.MapCoordinates = selectedUnitOriginalPosition;
        }

        public void GenerateTargetingGridAtUnit(SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = SelectedUnit.MapEntity.MapCoordinates;
            GenerateTargetingGridAtCoordinates(selectedUnitOriginalPosition, SelectedUnit.Stats.AtkRange, spriteAtlas);
        }

        public void GenerateTargetingGridAtCoordinates(Vector2 origin, int[] range, SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = origin;
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(mapContainer, spriteAtlas);
            unitTargetingContext.GenerateTargetingGrid(origin, range);
        }
    }
}
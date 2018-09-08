using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
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

        private readonly Dictionary<Direction, UnitSprite.UnitAnimationState> directionToAnimation =
            new Dictionary<Direction, UnitSprite.UnitAnimationState>
            {
                {Direction.Down, UnitSprite.UnitAnimationState.WalkDown},
                {Direction.Up, UnitSprite.UnitAnimationState.WalkUp},
                {Direction.Right, UnitSprite.UnitAnimationState.WalkRight},
                {Direction.Left, UnitSprite.UnitAnimationState.WalkLeft},
            };

        public MapUI MapUI { get; private set; }

        public MapContext(MapContainer mapContainer, MapUI mapUI)
        {
            this.mapContainer = mapContainer;
            MapUI = mapUI;
            CurrentTurnState = TurnState.SelectUnit;
            selectedUnitOriginalPosition = new Vector2();
        }

        public void UpdateWindows()
        {
            //Initiative Window
            MapUI.GenerateInitiativeWindow(GameContext.Units);

            //Turn Window
            //FIXME Stop hardcoding the X-Value of the Turn Window
            Vector2 turnWindowSize = new Vector2(290, MapUI.InitiativeWindow.Height);
            MapUI.GenerateTurnWindow(turnWindowSize);

            //Help Window TODO make this context-sensitive
            const string helpText = "HELP: Select a unit. Defeat the enemy!";
            MapUI.GenerateHelpWindow(helpText);
        }

        public void ProceedToNextState()
        {
            if (CurrentTurnState == TurnState.ResolvingTurn)
            {
                EndTurn();
            }
            else
            {
                CurrentTurnState++;
                Trace.WriteLine("Changing state: " + CurrentTurnState);
            }
        }

        public void EndTurn()
        {
            CurrentTurnState = TurnState.SelectUnit;
            Trace.WriteLine("Resetting to initial state: " + CurrentTurnState);
        }

        public void CancelMovement()
        {
            if (CurrentTurnState == TurnState.UnitMoving)
            {
                SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                ReturnUnitToOriginalPosition();
                MapContainer.ClearDynamicGrid();
                CurrentTurnState--;
            }
        }

        public void ResetCursorToActiveUnit()
        {
            if (GameContext.ActiveUnit.UnitEntity != null)
            {
                MapContainer.MapCursor.MapCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;
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
            if (MapUI.UserPromptWindow != null)
            {
                MapUI.UserPromptWindow.Visible = false;
            }
        }

        public void GenerateMoveGrid(Vector2 origin, int maximumDistance, SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = origin;
            UnitMovingContext unitMovingContext = new UnitMovingContext(spriteAtlas);
            unitMovingContext.GenerateMoveGrid(origin, maximumDistance, SelectedUnit);
        }

        public void MoveCursorAndSelectedUnitWithinMoveGrid(Direction direction)
        {
            if (TargetTileHasADynamicTile(direction))
            {
                SelectedUnit.MoveUnitInDirection(direction, MapContainer.MapGridSize);
                SelectedUnit.SetUnitAnimation(directionToAnimation[direction]);
                MapContainer.MapCursor.MoveCursorInDirection(direction);
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
            return OtherUnitExistsAtCoordinates(MapContainer.MapCursor.MapCoordinates);
        }

        public bool OtherUnitExistsAtCoordinates(Vector2 coordinates)
        {
            return UnitSelector.FindOtherUnitEntityAtCoordinates(coordinates, SelectedUnit.UnitEntity) != null;
        }

        public void ReturnUnitToOriginalPosition()
        {
            SelectedUnit.UnitEntity.MapCoordinates = selectedUnitOriginalPosition;
            MapContainer.MapCursor.MapCoordinates = selectedUnitOriginalPosition;
        }

        public bool TargetUnitIsLegal(GameUnit targetUnit)
        {
            return targetUnit != null
                   && SelectedUnit != targetUnit
                   && BattleContext.CoordinatesAreInRange(SelectedUnit.UnitEntity.MapCoordinates,
                       targetUnit.UnitEntity.MapCoordinates, SelectedUnit.Stats.AtkRange)
                   && SelectedUnit.UnitTeam != targetUnit.UnitTeam;
        }

        public void GenerateTargetingGridAtUnit(SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = SelectedUnit.UnitEntity.MapCoordinates;
            GenerateTargetingGridAtCoordinates(selectedUnitOriginalPosition, SelectedUnit.Stats.AtkRange, spriteAtlas);
        }

        public void GenerateTargetingGridAtCoordinates(Vector2 origin, int[] range, SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = origin;
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(spriteAtlas);
            unitTargetingContext.GenerateTargetingGrid(origin, range);
        }
    }
}
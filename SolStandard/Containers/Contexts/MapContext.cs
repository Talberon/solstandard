using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

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
        private const string HelpText = "OBJECTIVE: Slay the enemy Monarch!";
        public GameMapUI GameMapUI { get; private set; }

        private readonly Dictionary<Direction, UnitSprite.UnitAnimationState> directionToAnimation =
            new Dictionary<Direction, UnitSprite.UnitAnimationState>
            {
                {Direction.Down, UnitSprite.UnitAnimationState.WalkDown},
                {Direction.Up, UnitSprite.UnitAnimationState.WalkUp},
                {Direction.Right, UnitSprite.UnitAnimationState.WalkRight},
                {Direction.Left, UnitSprite.UnitAnimationState.WalkLeft},
            };

        public MapContext(MapContainer mapContainer, GameMapUI gameMapUI)
        {
            this.mapContainer = mapContainer;
            GameMapUI = gameMapUI;
            CurrentTurnState = TurnState.SelectUnit;
            selectedUnitOriginalPosition = new Vector2();
        }

        public void UpdateWindowsEachTurn()
        {
            //Initiative Window
            GameMapUI.GenerateInitiativeWindow(GameContext.Units);

            //Turn Window
            //FIXME Stop hardcoding the X-Value of the Turn Window
            Vector2 turnWindowSize = new Vector2(290, GameMapUI.InitiativeWindow.Height);
            GameMapUI.GenerateTurnWindow(turnWindowSize);

            //Help Window
            GameMapUI.GenerateHelpWindow(HelpText);
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

        public void RevertToPreviousState()
        {
            if (CurrentTurnState <= TurnState.SelectUnit) return;

            CurrentTurnState--;
            Trace.WriteLine("Changing state: " + CurrentTurnState);
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

        public void SnapCursorToActiveUnit()
        {
            if (GameContext.ActiveUnit.UnitEntity != null)
            {
                MapContainer.MapCursor.SnapCursorToCoordinates(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            }
        }

        public void SlideCursorToActiveUnit()
        {
            if (GameContext.ActiveUnit.UnitEntity != null)
            {
                MapContainer.MapCursor.MoveCursorToCoordinates(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
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
                    new RenderText(AssetManager.PromptFont, promptText),
                    ButtonIconProvider.GetButton(ButtonIcon.A,
                        new Vector2(AssetManager.PromptFont.MeasureString("A").Y))
                }
            };
            WindowContentGrid promptWindowContentGrid = new WindowContentGrid(promptTextContent, 2);
            GameMapUI.GenerateUserPromptWindow(promptWindowContentGrid,
                new Vector2(0, 150));
        }

        public void ConfirmPromptWindow()
        {
            if (GameMapUI.UserPromptWindow != null)
            {
                GameMapUI.UserPromptWindow.Visible = false;
            }
        }

        public void GenerateMoveGrid(Vector2 origin, int maximumDistance, SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = origin;
            UnitMovingContext unitMovingContext = new UnitMovingContext(spriteAtlas);
            unitMovingContext.GenerateMoveGrid(origin, maximumDistance);
        }

        public void MoveCursorAndSelectedUnitWithinMoveGrid(Direction direction)
        {
            if (TargetTileHasADynamicTile(direction))
            {
                SelectedUnit.MoveUnitInDirection(direction, MapContainer.MapGridSize);
                SelectedUnit.SetUnitAnimation(directionToAnimation[direction]);
                MapContainer.MapCursor.MoveCursorInDirection(direction);
                AssetManager.MapUnitMoveSFX.Play();
            }
        }

        public void UpdateUnitPortraitWindows(MapSlice hoverTiles)
        {
            GameUnit hoverMapUnit = UnitSelector.SelectUnit(hoverTiles.UnitEntity);

            if (CurrentTurnState != TurnState.SelectUnit)
            {
                if (hoverMapUnit != GameContext.ActiveUnit)
                {
                    GameMapUI.UpdateRightPortraitAndDetailWindows(hoverMapUnit);
                }
                else
                {
                    GameMapUI.UpdateRightPortraitAndDetailWindows(null);
                }
            }
            else
            {
                GameMapUI.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                GameMapUI.UpdateRightPortraitAndDetailWindows(null);
            }

            //Terrain (Entity) Window
            GameMapUI.GenerateTerrainWindow(hoverTiles.TerrainEntity);
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

        public static bool CoordinatesWithinMapBounds(Vector2 coordinates)
        {
            if (coordinates.X > MapContainer.GameGrid[0].GetLength(0) - 1) return false;
            if (coordinates.X < 0) return false;
            if (coordinates.Y > MapContainer.GameGrid[0].GetLength(1) - 1) return false;
            if (coordinates.Y < 0) return false;

            return true;
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

        public void MoveActionMenuCursor(VerticalMenu.MenuCursorDirection direction)
        {
            GameMapUI.ActionMenu.MoveMenuCursor(direction);
            GameMapUI.GenerateActionMenuDescription();
        }

    }
}
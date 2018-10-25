using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Controller;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using GameMapUI = SolStandard.Containers.Controller.GameMapController;

namespace SolStandard.Containers.Contexts
{
    public class GameMapContext
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

        public TurnState CurrentTurnState { get; set; }
        public GameUnit SelectedUnit { get; set; }
        private Vector2 selectedUnitOriginalPosition;
        private readonly MapContainer mapContainer;
        private const string HelpText = "Select a unit.";
        public static GameMapUI GameMapController { get; private set; }
        public PauseMenuUI PauseMenuUI { get; private set; }

        private readonly Dictionary<Direction, UnitSprite.UnitAnimationState> directionToAnimation =
            new Dictionary<Direction, UnitSprite.UnitAnimationState>
            {
                {Direction.Down, UnitSprite.UnitAnimationState.WalkDown},
                {Direction.Up, UnitSprite.UnitAnimationState.WalkUp},
                {Direction.Right, UnitSprite.UnitAnimationState.WalkRight},
                {Direction.Left, UnitSprite.UnitAnimationState.WalkLeft},
            };

        public GameMapContext(MapContainer mapContainer, GameMapController gameMapController)
        {
            this.mapContainer = mapContainer;
            GameMapController = gameMapController;
            CurrentTurnState = TurnState.SelectUnit;
            selectedUnitOriginalPosition = new Vector2();
            PauseMenuUI = new PauseMenuUI(this);
        }

        public static void UpdateWindowsEachTurn()
        {
            //Initiative Window
            GameMapController.GenerateInitiativeWindow(GameContext.Units);

            //Turn Window
            GameMapController.GenerateTurnWindow();

            //Help Window
            GameMapController.GenerateHelpWindow(HelpText);
            GameMapController.GenerateObjectiveWindow();
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
                MapContainer.ClearDynamicAndPreviewGrids();
                CurrentTurnState--;
            }
        }

        public void ResetCursorToActiveUnit()
        {
            if (GameContext.ActiveUnit.UnitEntity != null)
            {
                MapContainer.MapCursor.SnapCursorToCoordinates(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            }
        }

        public MapContainer MapContainer
        {
            get { return mapContainer; }
        }

        public static void SetPromptWindowText(string promptText)
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
            GameMapController.GenerateUserPromptWindow(promptWindowContentGrid, new Vector2(0, 150));
        }

        public void ConfirmPromptWindow()
        {
            GameMapController.ClosePromptWindow();
        }

        public void GenerateMoveGrid(Vector2 origin, GameUnit selectedUnit, SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = origin;
            UnitMovingContext unitMovingContext = new UnitMovingContext(spriteAtlas);
            unitMovingContext.GenerateMoveGrid(origin, selectedUnit);
        }

        public void MoveCursorOnMap(Direction direction)
        {
            MapContainer.MapCursor.MoveCursorInDirection(direction);
        }

        public void MoveCursorAndSelectedUnitWithinMoveGrid(Direction direction)
        {
            MapContainer.MapCursor.MoveCursorInDirection(direction);

            if (MapContainer.GetMapSliceAtCursor().DynamicEntity != null)
            {
                SelectedUnit.MoveUnitToCoordinates(MapContainer.MapCursor.MapCoordinates);
                SelectedUnit.SetUnitAnimation(directionToAnimation[direction]);
                AssetManager.MapUnitMoveSFX.Play();

                MapContainer.ClearPreviewGrid();
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack), false)
                    .GenerateTargetingGrid(SelectedUnit.UnitEntity.MapCoordinates, SelectedUnit.Stats.AtkRange,
                        Layer.Preview);
            }
        }

        public void UpdateHoverContextWindows(MapSlice hoverSlice)
        {
            GameUnit hoverMapUnit = UnitSelector.SelectUnit(hoverSlice.UnitEntity);

            if (CurrentTurnState != TurnState.SelectUnit)
            {
                if (hoverMapUnit != GameContext.ActiveUnit)
                {
                    GameMapController.UpdateRightPortraitAndDetailWindows(hoverMapUnit);
                }
                else
                {
                    GameMapController.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                    GameMapController.UpdateRightPortraitAndDetailWindows(null);
                }
            }
            else
            {
                MapContainer.ClearDynamicAndPreviewGrids();
                if (hoverMapUnit != null)
                {
                    new UnitTargetingContext(
                            MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
                            false
                        )
                        .GenerateThreatGrid(hoverSlice.MapCoordinates, hoverMapUnit);
                }

                GameMapController.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                GameMapController.UpdateRightPortraitAndDetailWindows(null);
            }

            //Terrain (Entity) Window
            GameMapController.GenerateEntityWindow(hoverSlice);
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
            GameMapController.ActionMenu.MoveMenuCursor(direction);
            GameMapController.GenerateActionMenuDescription();

            GenerateActionPreviewGrid();
        }

        public void SelectActionMenuOption()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            GameMapController.ActionMenu.CurrentOption.Execute();
            GameMapController.ClearCombatMenu();

            ProceedToNextState();
            SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            AssetManager.MapUnitSelectSFX.Play();
        }

        public void MovePauseMenuCursor(VerticalMenu.MenuCursorDirection direction)
        {
            PauseMenuUI.CurrentMenu.MoveMenuCursor(direction);
        }

        public void SelectPauseMenuOption()
        {
            PauseMenuUI.CurrentMenu.SelectOption();
        }

        public void GenerateActionPreviewGrid()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            SkillOption skillOption = GameMapController.ActionMenu.CurrentOption as SkillOption;
            if (skillOption != null)
            {
                skillOption.Action.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates, Layer.Preview);
            }
        }
    }
}
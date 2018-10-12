using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
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

        public TurnState CurrentTurnState { get; set; }
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
            Vector2 turnWindowSize = new Vector2(300, GameMapUI.InitiativeWindow.Height);
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
                MapContainer.ClearDynamicAndPreviewGrids();
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
                MapContainer.MapCursor.SlideCursorToCoordinates(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
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
            GameMapUI.GenerateUserPromptWindow(promptWindowContentGrid, new Vector2(0, 150));
        }

        public void ConfirmPromptWindow()
        {
            if (GameMapUI.UserPromptWindow != null)
            {
                GameMapUI.UserPromptWindow.Visible = false;
            }
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
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
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
                    GameMapUI.UpdateRightPortraitAndDetailWindows(hoverMapUnit);
                }
                else
                {
                    GameMapUI.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                    GameMapUI.UpdateRightPortraitAndDetailWindows(null);
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

                GameMapUI.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                GameMapUI.UpdateRightPortraitAndDetailWindows(null);
            }

            //Terrain (Entity) Window
            GameMapUI.GenerateEntityWindow(hoverSlice);
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

            GenerateActionPreviewGrid();
        }

        public void GenerateActionPreviewGrid()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            SkillOption skillOption = GameMapUI.ActionMenu.CurrentOption as SkillOption;
            if (skillOption != null)
            {
                skillOption.Action.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates, Layer.Preview);
            }
        }
    }
}
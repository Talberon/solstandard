using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

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
        public GameUnit SelectedUnit { get; private set; }
        private Vector2 selectedUnitOriginalPosition;
        private readonly MapContainer mapContainer;
        private const string HelpText = "Select a unit.";
        public static GameMapView GameMapView { get; private set; }
        public PauseScreenView PauseScreenView { get; private set; }
        public int TurnCounter { get; private set; }
        public int RoundCounter { get; private set; }

        private readonly Dictionary<Direction, UnitAnimationState> directionToAnimation =
            new Dictionary<Direction, UnitAnimationState>
            {
                {Direction.Down, UnitAnimationState.WalkDown},
                {Direction.Up, UnitAnimationState.WalkUp},
                {Direction.Right, UnitAnimationState.WalkRight},
                {Direction.Left, UnitAnimationState.WalkLeft},
            };

        public GameMapContext(MapContainer mapContainer, GameMapView gameMapController)
        {
            this.mapContainer = mapContainer;
            GameMapView = gameMapController;
            CurrentTurnState = TurnState.SelectUnit;
            selectedUnitOriginalPosition = new Vector2();
            PauseScreenView = new PauseScreenView();
            TurnCounter = 1;
            RoundCounter = 1;
        }

        public static void UpdateWindowsEachTurn()
        {
            //Initiative Window
            GameMapView.GenerateInitiativeWindow(GameContext.Units);

            //Turn Window
            GameMapView.GenerateTurnWindow();

            //Help Window
            GameMapView.GenerateHelpWindow(HelpText);
            GameMapView.GenerateObjectiveWindow();
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

        public void ResolveTurn()
        {
            GameContext.Scenario.CheckForWinState();
            ConfirmPromptWindow();
            GameContext.ActiveUnit.DisableExhaustedUnit();
            GameContext.InitiativeContext.PassTurnToNextUnit();
            GameContext.ActiveUnit.ActivateUnit();
            UpdateWindowsEachTurn();
            ResetCursorToActiveUnit();
            MapContainer.MapCamera.CenterCameraToCursor();

            EndTurn();

            UpdateTurnCounters();

            ActivateEffectTiles();

            if (!GameContext.Units.TrueForAll(unit => unit.Stats.Hp <= 0))
            {
                EndTurnIfUnitIsDead();
            }

            GameContext.StatusScreenView.UpdateWindows();

            AssetManager.MapUnitSelectSFX.Play();
        }

        private void UpdateTurnCounters()
        {
            TurnCounter++;

            if (TurnCounter < GameContext.Units.Count) return;

            TurnCounter = 1;
            RoundCounter++;
        }


        private void EndTurnIfUnitIsDead()
        {
            if (CurrentTurnState == TurnState.SelectUnit && GameContext.ActiveUnit.UnitEntity == null)
            {
                ResolveTurn();
            }
        }

        private void RevertToPreviousState()
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

        public void FinishMoving()
        {
            if (OtherUnitExistsAtCursor() || MapContainer.GetMapSliceAtCursor().DynamicEntity == null)
            {
                MapContainer.AddNewToastAtMapCursor("Cannot end move on this space!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            ProceedToNextState();

            MapContainer.ClearDynamicAndPreviewGrids();
            SelectedUnit.SetUnitAnimation(UnitAnimationState.Idle);
            AssetManager.MapUnitSelectSFX.Play();

            GenerateActionMenu();
            GenerateActionPreviewGrid();
        }

        public void ResetCursorToActiveUnit()
        {
            if (GameContext.ActiveUnit.UnitEntity != null)
            {
                MapContainer.MapCursor.SnapCursorToCoordinates(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
                MapContainer.MapCamera.CenterCameraToCursor();
            }
        }

        public void CancelMove()
        {
            if (CurrentTurnState == TurnState.UnitMoving)
            {
                SelectedUnit.SetUnitAnimation(UnitAnimationState.Idle);
                ReturnUnitToOriginalPosition();
                MapContainer.ClearDynamicAndPreviewGrids();
                CurrentTurnState--;
            }

            AssetManager.MapUnitCancelSFX.Play();
        }

        public void CancelAction()
        {
            GameContext.ActiveUnit.CancelArmedSkill();
            ResetCursorToActiveUnit();
            GenerateActionMenu();
            RevertToPreviousState();
        }

        private void StartMoving()
        {
            if (SelectedUnit != null)
            {
                Trace.WriteLine("Selecting unit: " + SelectedUnit.Team + " " +
                                SelectedUnit.Role);
                ProceedToNextState();
                GenerateMoveGrid(
                    MapContainer.MapCursor.MapCoordinates,
                    SelectedUnit,
                    new SpriteAtlas(
                        new Texture2DWrapper(AssetManager.ActionTiles.MonoGameTexture),
                        new Vector2(GameDriver.CellSize),
                        (int) MapDistanceTile.TileType.Movement
                    )
                );

                MapContainer.ClearPreviewGrid();
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack), false)
                    .GenerateTargetingGrid(
                        SelectedUnit.UnitEntity.MapCoordinates,
                        SelectedUnit.Stats.AtkRange,
                        Layer.Preview
                    );
            }
            else
            {
                Trace.WriteLine("No unit to select.");
            }
        }

        public void SelectUnitAndStartMoving()
        {
            if (!TrySelectUnit()) return;
            StartMoving();
            AssetManager.MapUnitSelectSFX.Play();
        }


        private bool TrySelectUnit()
        {
            //Select the unit. Store it somewhere.
            SelectedUnit = UnitSelector.SelectUnit(MapContainer.GetMapSliceAtCursor().UnitEntity);

            //If the entity selected isn't the active unit, don't select it.
            if (SelectedUnit != GameContext.ActiveUnit)
            {
                SelectedUnit = null;
                AssetManager.WarningSFX.Play();
                MapContainer.AddNewToastAtMapCursor("Not the active unit!", 50);
                return false;
            }

            return true;
        }

        public void ExecuteAction()
        {
            GameContext.ActiveUnit.ExecuteArmedSkill(MapContainer.GetMapSliceAtCursor());
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
            GameMapView.GenerateUserPromptWindow(promptWindowContentGrid, new Vector2(0, 150));
        }

        private static void ConfirmPromptWindow()
        {
            GameMapView.CloseUserPromptWindow();
        }

        private void GenerateMoveGrid(Vector2 origin, GameUnit selectedUnit, SpriteAtlas spriteAtlas)
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

            if (MapContainer.GetMapSliceAtCursor().DynamicEntity == null) return;

            SelectedUnit.MoveUnitToCoordinates(MapContainer.MapCursor.MapCoordinates);
            SelectedUnit.SetUnitAnimation(directionToAnimation[direction]);
            AssetManager.MapUnitMoveSFX.Play();

            MapContainer.ClearPreviewGrid();
            new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack), false)
                .GenerateTargetingGrid(SelectedUnit.UnitEntity.MapCoordinates, SelectedUnit.Stats.AtkRange,
                    Layer.Preview);
        }

        public void UpdateHoverContextWindows(MapSlice hoverSlice)
        {
            GameUnit hoverMapUnit = UnitSelector.SelectUnit(hoverSlice.UnitEntity);

            if (CurrentTurnState != TurnState.SelectUnit)
            {
                if (hoverMapUnit != GameContext.ActiveUnit)
                {
                    GameMapView.UpdateRightPortraitAndDetailWindows(hoverMapUnit);
                }
                else
                {
                    GameMapView.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                    GameMapView.UpdateRightPortraitAndDetailWindows(null);
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
                    ).GenerateThreatGrid(hoverSlice.MapCoordinates, hoverMapUnit);
                }

                GameMapView.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                GameMapView.UpdateRightPortraitAndDetailWindows(null);
            }

            //Terrain (Entity) Window
            GameMapView.GenerateEntityWindow(hoverSlice);
        }

        public static bool CoordinatesWithinMapBounds(Vector2 coordinates)
        {
            if (coordinates.X > MapContainer.GameGrid[0].GetLength(0) - 1) return false;
            if (coordinates.X < 0) return false;
            if (coordinates.Y > MapContainer.GameGrid[0].GetLength(1) - 1) return false;
            if (coordinates.Y < 0) return false;

            return true;
        }

        private bool OtherUnitExistsAtCursor()
        {
            return OtherUnitExistsAtCoordinates(MapContainer.MapCursor.MapCoordinates);
        }

        private bool OtherUnitExistsAtCoordinates(Vector2 coordinates)
        {
            return UnitSelector.FindOtherUnitEntityAtCoordinates(coordinates, SelectedUnit.UnitEntity) != null;
        }

        private void ReturnUnitToOriginalPosition()
        {
            SelectedUnit.UnitEntity.MapCoordinates = selectedUnitOriginalPosition;
            MapContainer.MapCursor.MapCoordinates = selectedUnitOriginalPosition;
        }

        public void MoveActionMenuCursor(VerticalMenu.MenuCursorDirection direction)
        {
            GameMapView.ActionMenu.MoveMenuCursor(direction);
            GameMapView.GenerateActionMenuDescription();

            GenerateActionPreviewGrid();
        }

        public void SelectActionMenuOption()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            GameMapView.ActionMenu.CurrentOption.Execute();
            GameMapView.CloseCombatMenu();

            ProceedToNextState();
            SelectedUnit.SetUnitAnimation(UnitAnimationState.Attack);
            AssetManager.MapUnitSelectSFX.Play();
        }

        private static void ActivateEffectTiles()
        {
            List<IEffectTile> effectTiles = MapContainer.GameGrid[(int) Layer.Entities].OfType<IEffectTile>().ToList();

            effectTiles.ForEach(tile => tile.TriggerEffect());

            foreach (IEffectTile effectTile in effectTiles)
            {
                if (effectTile.IsExpired)
                {
                    MapContainer.GameGrid[(int) Layer.Entities][(int) effectTile.MapCoordinates.X,
                        (int) effectTile.MapCoordinates.Y] = null;
                }
            }
        }

        public void MovePauseMenuCursor(VerticalMenu.MenuCursorDirection direction)
        {
            PauseScreenView.CurrentMenu.MoveMenuCursor(direction);
        }

        public void SelectPauseMenuOption()
        {
            PauseScreenView.CurrentMenu.SelectOption();
        }

        private void GenerateActionPreviewGrid()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            SkillOption skillOption = GameMapView.ActionMenu.CurrentOption as SkillOption;
            if (skillOption != null)
            {
                skillOption.Action.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates, Layer.Preview);
            }
        }

        private static void GenerateActionMenu()
        {
            Color windowColour = TeamUtility.DetermineTeamColor(GameContext.ActiveUnit.Team);

            MenuOption[] options = UnitContextualActionMenuContext.GenerateActionMenuOptions(windowColour);

            IRenderable cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height), 1);

            GameMapView.ActionMenu = new VerticalMenu(options, cursorSprite, windowColour);
            GameMapView.GenerateActionMenuDescription();
        }
    }
}
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
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
            GameMapView.GenerateInitiativeWindow();

            //Turn Window
            GameMapView.GenerateTurnWindow();

            //Help Window
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
            TriggerEffectTilesTurnEnd();
            GameContext.Scenario.CheckForWinState();
            UpdateUnitMorale(Team.Blue);
            UpdateUnitMorale(Team.Red);
            ConfirmPromptWindow();
            GameContext.InitiativeContext.PassTurnToNextUnit();
            UpdateWindowsEachTurn();
            ResetCursorToActiveUnit();

            EndTurn();
            UpdateTurnCounters();

            if (NotEveryUnitIsDead())
            {
                EndTurnIfUnitIsDead();
            }

            GameContext.StatusScreenView.UpdateWindows();

            if (GameContext.ActiveUnit.UnitEntity != null)
            {
                ExecuteAIActions();
            }
        }

        private static void UpdateUnitMorale(Team team)
        {
            List<GameUnit> teamUnits = GameContext.Units.Where(unit => unit.Team == team).ToList();

            bool hasLivingCommander = teamUnits.Any(unit => unit.IsCommander && unit.IsAlive);

            if (hasLivingCommander) return;

            Queue<IEvent> statusEvents = new Queue<IEvent>();

            IEnumerable<GameUnit> livingUnits = teamUnits.Where(unit => unit.IsAlive);
            foreach (GameUnit unit in livingUnits)
            {
                statusEvents.Enqueue(new CastStatusEffectEvent(unit, new MoraleBrokenStatus(99, unit)));
            }

            GlobalEventQueue.QueueEvents(statusEvents);
        }

        private static bool NotEveryUnitIsDead()
        {
            return !GameContext.Units.TrueForAll(unit => unit.Stats.CurrentHP <= 0);
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

            GameMapView.GenerateActionMenus();
            GameMapView.GenerateCurrentMenuDescription();

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

        public void CancelActionMenu()
        {
            if (CurrentTurnState == TurnState.UnitDecidingAction)
            {
                MapContainer.ClearDynamicAndPreviewGrids();
                GameMapView.CloseCombatMenu();

                RevertToPreviousState();
                CancelMove();
            }
        }

        public void CancelAction()
        {
            GameContext.ActiveUnit.CancelArmedSkill();
            ResetCursorToActiveUnit();
            GameMapView.GenerateActionMenus();
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
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateTargetingGrid(
                        SelectedUnit.UnitEntity.MapCoordinates,
                        SelectedUnit.Stats.CurrentAtkRange,
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

            if (GameContext.InitiativeContext.SelectActiveUnit(SelectedUnit)) return true;
            //If the entity selected isn't the active unit, don't select it.
            SelectedUnit = null;
            AssetManager.WarningSFX.Play();
            MapContainer.AddNewToastAtMapCursor("Not an active unit!", 50);
            return false;
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
            new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                .GenerateTargetingGrid(SelectedUnit.UnitEntity.MapCoordinates, SelectedUnit.Stats.CurrentAtkRange,
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
                if (hoverMapUnit != null && GameContext.ActiveUnit.Team != Team.Creep)
                {
                    new UnitTargetingContext(
                        MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack)
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
            GameMapView.CurrentMenu.MoveMenuCursor(direction);
            GameMapView.GenerateCurrentMenuDescription();

            GenerateActionPreviewGrid();
        }

        public void SelectActionMenuOption()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            GameMapView.CurrentMenu.CurrentOption.Execute();
            GameMapView.CloseCombatMenu();

            ProceedToNextState();
            SelectedUnit.SetUnitAnimation(UnitAnimationState.Attack);
            AssetManager.MapUnitSelectSFX.Play();
        }

        public void RefreshCurrentActionMenuOption()
        {
            GameMapView.CurrentMenu.CurrentOption.Refresh();
        }

        public void IncrementCurrentAdjustableAction(int value)
        {
            IIncrementableAction incrementableAction = CurrentIncrementableAction();
            if (incrementableAction != null) incrementableAction.Increment(value);
        }

        public void DecrementCurrentAdjustableAction(int value)
        {
            IIncrementableAction incrementableAction = CurrentIncrementableAction();
            if (incrementableAction != null) incrementableAction.Decrement(value);
        }

        private static IIncrementableAction CurrentIncrementableAction()
        {
            ActionOption currentActionOption = GameMapView.CurrentMenu.CurrentOption as ActionOption;
            if (currentActionOption == null) return null;
            return currentActionOption.Action as IIncrementableAction;
        }

        private void ExecuteAIActions()
        {
            GameContext.ActiveUnit.ExecuteRoutines();
        }

        public static void TriggerEffectTilesTurnStart()
        {
            List<IEffectTile> effectTiles = MapContainer.GameGrid[(int) Layer.Entities].OfType<IEffectTile>().ToList();

            if (effectTiles.Count <= 0) return;

            Queue<IEvent> startOfTurnEffectTileEvents = new Queue<IEvent>();
            startOfTurnEffectTileEvents.Enqueue(
                new ToastAtCoordinatesEvent(
                    MapCursor.CurrentPixelCoordinates,
                    "Resolving Tile Effects...",
                    AssetManager.MenuConfirmSFX,
                    100
                )
            );
            startOfTurnEffectTileEvents.Enqueue(new WaitFramesEvent(100));

            foreach (IEffectTile tile in effectTiles)
            {
                startOfTurnEffectTileEvents.Enqueue(new TriggerEffectTileEvent(tile, EffectTriggerTime.StartOfTurn));
            }

            startOfTurnEffectTileEvents.Enqueue(new RemoveExpiredEffectTilesEvent(effectTiles));

            GlobalEventQueue.QueueEvents(startOfTurnEffectTileEvents);
        }

        private static void TriggerEffectTilesTurnEnd()
        {
            List<IEffectTile> effectTiles = MapContainer.GameGrid[(int) Layer.Entities].OfType<IEffectTile>().ToList();

            if (effectTiles.Count <= 0) return;

            Queue<IEvent> endOfTurnEffectTileEvents = new Queue<IEvent>();
            foreach (IEffectTile tile in effectTiles)
            {
                endOfTurnEffectTileEvents.Enqueue(
                    new TriggerEffectTileEvent(tile, EffectTriggerTime.EndOfTurn)
                );
            }

            endOfTurnEffectTileEvents.Enqueue(new RemoveExpiredEffectTilesEvent(effectTiles));
            GlobalEventQueue.QueueEvents(endOfTurnEffectTileEvents);
        }

        public static void RemoveExpiredEffectTiles(IEnumerable<IEffectTile> effectTiles)
        {
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

        public void ToggleCombatMenu()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            GameMapView.ToggleCombatMenu();
            GameMapView.GenerateCurrentMenuDescription();
            GenerateActionPreviewGrid();
            AssetManager.MenuMoveSFX.Play();
        }

        private static void GenerateActionPreviewGrid()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            ActionOption actionOption = GameMapView.CurrentMenu.CurrentOption as ActionOption;
            if (actionOption != null)
            {
                actionOption.Action.GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates, Layer.Preview);
            }
        }
    }
}
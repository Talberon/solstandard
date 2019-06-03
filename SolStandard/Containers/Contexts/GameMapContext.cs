using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
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
            ResolvingTurn,
            AdHocDraft
        }

        public TurnState CurrentTurnState { get; set; }
        public GameUnit SelectedUnit { get; private set; }
        private Vector2 selectedUnitOriginalPosition;
        private readonly MapContainer mapContainer;
        public static GameMapView GameMapView { get; private set; }
        public PauseScreenView PauseScreenView { get; private set; }
        public int TurnCounter { get; private set; }
        public int RoundCounter { get; private set; }
        public bool CanCancelAction { get; set; }
        private GameUnit HoverUnit { get; set; }

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
            CanCancelAction = true;
        }

        public static void UpdateWindowsEachTurn()
        {
            //Initiative Window
            GameMapView.GenerateInitiativeWindow();

            //Help Window
            GameMapView.GenerateObjectiveWindow();
        }

        public void ResolveTurn()
        {
            if (GameContext.CurrentGameState == GameContext.GameState.Results) return;
            
            TriggerEffectTilesTurnEnd();
            GameContext.Scenario.CheckForWinState();
            UpdateUnitMorale(Team.Blue);
            UpdateUnitMorale(Team.Red);
            ConfirmPromptWindow();
            GameContext.InitiativeContext.PassTurnToNextUnit();
            UpdateWindowsEachTurn();
            ResetCursorToActiveUnit();

            ResetTurnState();
            UpdateTurnCounters();

            if (NotEveryUnitIsDead())
            {
                EndTurnIfUnitIsDead();
            }

            GameContext.StatusScreenView.UpdateWindows();

            StartTurn();
        }


        public static void FinishTurn(bool skipProcs)
        {
            MapContainer.ClearDynamicAndPreviewGrids();

            if (GameContext.GameMapContext.SelectedUnit != null)
            {
                GameContext.GameMapContext.SelectedUnit.SetUnitAnimation(UnitAnimationState.Idle);

                if (!skipProcs)
                {
                    IEnumerable<ITurnProc> activeUnitTurnProcs = GameContext.ActiveUnit.StatusEffects
                        .Where(effect => effect is ITurnProc)
                        .Cast<ITurnProc>();

                    foreach (ITurnProc turnProc in activeUnitTurnProcs)
                    {
                        turnProc.OnTurnEnd();
                    }
                }
            }

            SetPromptWindowText("Confirm End Turn");
            GameContext.GameMapContext.CurrentTurnState = TurnState.ResolvingTurn;
        }

        private void StartTurn()
        {
            CanCancelAction = true;

            if (GameContext.GameMapContext.SelectedUnit == null) return;

            IEnumerable<ITurnProc> activeUnitTurnProcs = GameContext.ActiveUnit.StatusEffects
                .Where(effect => effect is ITurnProc)
                .Cast<ITurnProc>();

            foreach (ITurnProc turnProc in activeUnitTurnProcs)
            {
                turnProc.OnTurnStart();
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

        public void ResetTurnState()
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

            CurrentTurnState = TurnState.UnitDecidingAction;

            MapContainer.ClearDynamicAndPreviewGrids();
            SelectedUnit.SetUnitAnimation(UnitAnimationState.Idle);
            AssetManager.MapUnitSelectSFX.Play();

            ShowActionMenu();
        }

        private static void ShowActionMenu()
        {
            GameMapView.GenerateActionMenus();
            GameMapView.GenerateCurrentMenuDescription();

            GenerateActionPreviewGrid();
        }

        public void ResetCursorToNextUnitOnTeam()
        {
            GameContext.InitiativeContext.SelectNextUnitOnActiveTeam();

            if (GameContext.ActiveUnit.UnitEntity == null) return;

            MapContainer.MapCursor.SnapCursorToCoordinates(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            MapContainer.MapCamera.CenterCameraToCursor();
        }

        public void ResetCursorToPreviousUnitOnTeam()
        {
            GameContext.InitiativeContext.SelectPreviousUnitOnActiveTeam();

            if (GameContext.ActiveUnit.UnitEntity == null) return;

            MapContainer.MapCursor.SnapCursorToCoordinates(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            MapContainer.MapCamera.CenterCameraToCursor();
        }


        public void ResetCursorToActiveUnit()
        {
            if (GameContext.ActiveUnit.UnitEntity == null) return;

            MapContainer.MapCursor.SnapCursorToCoordinates(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
            MapContainer.MapCamera.CenterCameraToCursor();
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
            if (CanCancelAction)
            {
                if (CurrentTurnState != TurnState.UnitDecidingAction) return;

                MapContainer.ClearDynamicAndPreviewGrids();
                GameMapView.CloseCombatMenu();

                RevertToPreviousState();
                CancelMove();
            }
            else
            {
                CancelExtraAction();
            }
        }

        public void CancelTargetAction()
        {
            if (CanCancelAction)
            {
                GameContext.ActiveUnit.CancelArmedSkill();
                ResetCursorToActiveUnit();
                GameMapView.GenerateActionMenus();
                RevertToPreviousState();
            }
            else
            {
                CancelExtraAction();
            }
        }

        private void CancelExtraAction()
        {
            if (CurrentTurnState == TurnState.UnitTargeting)
            {
                GameContext.GameMapContext.ResetToActionMenu();
                AssetManager.MapUnitCancelSFX.Play();
            }
            else
            {
                MapContainer.AddNewToastAtMapCursor("Can't cancel action!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private void StartMoving()
        {
            if (SelectedUnit != null)
            {
                Trace.WriteLine("Selecting unit: " + SelectedUnit.Team + " " + SelectedUnit.Role);
                CurrentTurnState = TurnState.UnitMoving;
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


        public void ResetToActionMenu()
        {
            AssetManager.SkillBuffSFX.Play();
            CurrentTurnState = TurnState.UnitDecidingAction;
            CanCancelAction = false;
            ShowActionMenu();
            ConfirmPromptWindow();
        }

        private static void ConfirmPromptWindow()
        {
            GameMapView.CloseUserPromptWindow();
        }

        private void GenerateMoveGrid(Vector2 origin, GameUnit selectedUnit, SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = origin;
            UnitMovingContext unitMovingContext = new UnitMovingContext(spriteAtlas);
            unitMovingContext.GenerateMoveGrid(origin, selectedUnit.Stats.Mv, selectedUnit.Team);
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
        }

        public void UpdateUnitAttackRangePreview()
        {
            MapContainer.ClearPreviewGrid();
            new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                .GenerateTargetingGrid(SelectedUnit.UnitEntity.MapCoordinates, SelectedUnit.Stats.CurrentAtkRange,
                    Layer.Preview);
        }

        public void UpdateHoverContextWindows()
        {
            MapSlice hoverSlice = MapContainer.GetMapSliceAtCursor();

            GameUnit hoverMapUnit = UnitSelector.SelectUnit(hoverSlice.UnitEntity);

            if (CurrentTurnState != TurnState.SelectUnit)
            {
                if (hoverMapUnit != GameContext.ActiveUnit)
                {
                    GameMapView.UpdateLeftPortraitAndDetailWindows(GameContext.ActiveUnit);
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
                if (hoverMapUnit != null && GameContext.ActiveUnit.Team != Team.Creep)
                {
                    if (MapContainer.GetMapElementsFromLayer(Layer.Dynamic).Count == 0 || HoverUnit != hoverMapUnit)
                    {
                        MapContainer.ClearDynamicAndPreviewGrids();
                        new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                            .GenerateThreatGrid(hoverSlice.MapCoordinates, hoverMapUnit, hoverMapUnit.Team);
                    }
                }
                else if (hoverSlice.TerrainEntity is IThreatRange)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    IThreatRange entityThreat = (IThreatRange) hoverSlice.TerrainEntity;
                    new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                        .GenerateThreatGrid(hoverSlice.MapCoordinates, entityThreat);
                }
                else
                {
                    MapContainer.ClearDynamicAndPreviewGrids();
                }

                GameMapView.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                GameMapView.UpdateRightPortraitAndDetailWindows(null);
            }

            //Terrain (Entity) Window
            GameMapView.SetEntityWindow(hoverSlice);

            HoverUnit = hoverMapUnit;
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

        public void MoveActionMenuCursor(MenuCursorDirection direction)
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

            CurrentTurnState = TurnState.UnitTargeting;
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

        public static void TriggerEffectTilesTurnStart()
        {
            List<IEffectTile> effectTiles = MapContainer.GameGrid[(int) Layer.Entities].OfType<IEffectTile>().ToList();

            if (effectTiles.Count <= 0) return;

            Queue<IEvent> startOfTurnEffectTileEvents = new Queue<IEvent>();
            startOfTurnEffectTileEvents.Enqueue(
                new ToastAtCursorEvent(
                    "Resolving Tile Effects...",
                    AssetManager.MenuConfirmSFX,
                    100
                )
            );
            startOfTurnEffectTileEvents.Enqueue(new WaitFramesEvent(50));

            foreach (IEffectTile tile in effectTiles.Where(tile => tile.WillTrigger(EffectTriggerTime.StartOfTurn)))
            {
                startOfTurnEffectTileEvents.Enqueue(new TriggerEffectTileEvent(tile, EffectTriggerTime.StartOfTurn,
                    50));
            }

            startOfTurnEffectTileEvents.Enqueue(new RemoveExpiredEffectTilesEvent(effectTiles));

            GlobalEventQueue.QueueEvents(startOfTurnEffectTileEvents);
        }

        private static void TriggerEffectTilesTurnEnd()
        {
            List<IEffectTile> effectTiles = MapContainer.GameGrid[(int) Layer.Entities].OfType<IEffectTile>().ToList();

            if (effectTiles.Count <= 0) return;

            Queue<IEvent> endOfTurnEffectTileEvents = new Queue<IEvent>();
            foreach (IEffectTile tile in effectTiles.Where(tile => tile.WillTrigger(EffectTriggerTime.EndOfTurn)))
            {
                endOfTurnEffectTileEvents.Enqueue(
                    new TriggerEffectTileEvent(tile, EffectTriggerTime.EndOfTurn, 80)
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

        public void MovePauseMenuCursor(MenuCursorDirection direction)
        {
            PauseScreenView.CurrentMenu.MoveMenuCursor(direction);
        }

        public void SelectPauseMenuOption()
        {
            PauseScreenView.CurrentMenu.SelectOption();
        }

        public void OpenDraftMenu()
        {
            CurrentTurnState = TurnState.AdHocDraft;
            GameMapView.GenerateDraftMenu(GameContext.ActiveUnit.Team);
            AssetManager.MenuConfirmSFX.Play();
        }

        public void MoveDraftMenuCursor(MenuCursorDirection direction)
        {
            GameMapView.AdHocDraftMenu.MoveMenuCursor(direction);
        }

        public void SelectDraftMenuOption()
        {
            GameMapView.AdHocDraftMenu.SelectOption();
        }

        public void ClearDraftMenu()
        {
            GameMapView.CloseAdHocDraftMenu();
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

        public void ShowUnitCodexEntry()
        {
            MapSlice currentSlice = MapContainer.GetMapSliceAtCursor();

            GameUnit selectedUnit = UnitSelector.SelectUnit(currentSlice.UnitEntity);

            if (selectedUnit == null) return;

            GameContext.CodexContext.OpenMenu();
            GameContext.CodexContext.ShowUnitDetails(selectedUnit);
            AssetManager.MapUnitSelectSFX.Play();
        }

        public void ToggleItemPreview()
        {
            if (ShowingItemPreview)
            {
                AssetManager.MapUnitCancelSFX.Play();
                GameMapView.CloseItemDetailWindow();
                GameContext.CurrentGameState = GameContext.GameState.InGame;
                return;
            }

            MapSlice currentSlice = MapContainer.GetMapSliceAtCursor();

            List<IItem> items = CollectItemsFromSlice(currentSlice);

            if (items.Count > 0)
            {
                AssetManager.MapUnitSelectSFX.Play();
                GameMapView.GenerateItemDetailWindow(items);
                GameContext.CurrentGameState = GameContext.GameState.ItemPreview;
            }
            else
            {
                AssetManager.WarningSFX.Play();
                MapContainer.AddNewToastAtMapCursor("No items to preview!", 50);
            }
        }

        public static List<IItem> CollectItemsFromSlice(MapSlice currentSlice)
        {
            List<IItem> items = new List<IItem>();

            Spoils spoils = currentSlice.ItemEntity as Spoils;
            if (spoils != null)
            {
                items.AddRange(spoils.Items);
            }
            else
            {
                IItem item = currentSlice.ItemEntity as IItem;
                if (item != null) items.Add(item);
            }

            Vendor vendor = currentSlice.TerrainEntity as Vendor;
            if (vendor != null)
            {
                items.AddRange(vendor.Items);
            }

            GameUnit sliceUnit = UnitSelector.SelectUnit(currentSlice.UnitEntity);
            if (sliceUnit != null)
            {
                sliceUnit.Inventory.ForEach(item => items.Add(item));
            }

            return items;
        }

        private static bool ShowingItemPreview
        {
            get { return GameMapView.ItemDetailWindow != null; }
        }
    }
}
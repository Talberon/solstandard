using System;
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
using SolStandard.Utility.Buttons;
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
        public static GameMapView GameMapView { get; private set; }
        public MapContainer MapContainer { get; }
        public int TurnCounter { get; private set; }
        public int RoundCounter { get; private set; }
        public bool CanCancelAction { get; set; }
        private GameUnit HoverUnit { get; set; }
        private TerrainEntity LastHoverEntity { get; set; }
        private TerrainEntity LastHoverItem { get;set; }

        private readonly Dictionary<Direction, UnitAnimationState> directionToAnimation =
            new Dictionary<Direction, UnitAnimationState>
            {
                {Direction.Down, UnitAnimationState.WalkDown},
                {Direction.Up, UnitAnimationState.WalkUp},
                {Direction.Right, UnitAnimationState.WalkRight},
                {Direction.Left, UnitAnimationState.WalkLeft}
            };

        public GameMapContext(MapContainer mapContainer, GameMapView gameMapController)
        {
            MapContainer = mapContainer;
            GameMapView = gameMapController;
            CurrentTurnState = TurnState.SelectUnit;
            selectedUnitOriginalPosition = new Vector2();
            TurnCounter = 1;
            RoundCounter = 1;
            CanCancelAction = true;
        }

        public void PlayAnimationAtCoordinates(TriggeredAnimation animation, Vector2 coordinates)
        {
            animation.PlayOnce();
            MapContainer.GameGrid[(int) Layer.OverlayEffect][(int) coordinates.X, (int) coordinates.Y] =
                new Decoration("Interaction", "Decoration", animation, coordinates);
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

            GameContext.Scenario.CheckForWinState();
            UpdateUnitMorale(Team.Blue);
            UpdateUnitMorale(Team.Red);
            ConfirmPromptWindow();
            GameContext.InitiativeContext.PassTurnToNextUnit();
            UpdateWindowsEachTurn();

            ResetTurnState();
            UpdateTurnCounters();

            if (NotEveryUnitIsDead())
            {
                EndTurnIfUnitIsDead();
            }

            GameContext.StatusScreenView.UpdateWindows();

            StartTurn();
            ResetCursorToActiveUnit();
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
                GameContext.ActiveUnit.CancelArmedSkill();
                ResetCursorToActiveUnit();
                ResetToActionMenu();
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
                        GameDriver.CellSizeVector,
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


        public static void SetPromptWindowText(string promptText)
        {
            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(AssetManager.PromptFont, promptText),
                    InputIconProvider.GetInputIcon(Input.Confirm,
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

            MapSlice mapSliceAtCursor = MapContainer.GetMapSliceAtCursor();
            if (mapSliceAtCursor.DynamicEntity == null) return;

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
                UpdateThreatRangePreview(hoverMapUnit, hoverSlice);

                GameMapView.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                GameMapView.UpdateRightPortraitAndDetailWindows(null);
            }

            //Terrain (Entity) Window
            GameMapView.SetEntityWindow(hoverSlice);

            HoverUnit = hoverMapUnit;
            LastHoverEntity = hoverSlice.TerrainEntity;
            LastHoverItem = hoverSlice.ItemEntity;
        }

        private void UpdateThreatRangePreview(GameUnit hoverMapUnit, MapSlice hoverSlice)
        {
            if (hoverMapUnit != null && GameContext.ActiveUnit.Team != Team.Creep)
            {
                if (MapContainer.GetMapElementsFromLayer(Layer.Dynamic).Count != 0 && HoverUnit == hoverMapUnit) return;

                MapContainer.ClearDynamicAndPreviewGrids();
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateThreatGrid(hoverSlice.MapCoordinates, hoverMapUnit, hoverMapUnit.Team);
            }
            else if (hoverSlice.TerrainEntity is IThreatRange entityThreat)
            {
                if (LastHoverEntity == hoverSlice.TerrainEntity && LastHoverItem == hoverSlice.ItemEntity) return;
                MapContainer.ClearDynamicAndPreviewGrids();
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateThreatGrid(hoverSlice.MapCoordinates, entityThreat);
            }
            else
            {
                MapContainer.ClearDynamicAndPreviewGrids();
            }
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
            SelectedUnit.UnitEntity.SnapToCoordinates(selectedUnitOriginalPosition);
            MapContainer.MapCursor.SnapCursorToCoordinates(selectedUnitOriginalPosition);
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
            SelectedUnit.SetUnitAnimation(UnitAnimationState.Active);
            AssetManager.MapUnitSelectSFX.Play();
        }

        public void RefreshCurrentActionMenuOption()
        {
            GameMapView.CurrentMenu.CurrentOption.Refresh();
        }

        public void IncrementCurrentAdjustableAction(int value)
        {
            IIncrementableAction incrementableAction = CurrentIncrementableAction();
            incrementableAction?.Increment(value);
        }

        public void DecrementCurrentAdjustableAction(int value)
        {
            IIncrementableAction incrementableAction = CurrentIncrementableAction();
            incrementableAction?.Decrement(value);
        }

        private static IIncrementableAction CurrentIncrementableAction()
        {
            if (!(GameMapView.CurrentMenu.CurrentOption is ActionOption currentActionOption)) return null;
            return currentActionOption.Action as IIncrementableAction;
        }

        /// <summary>
        /// Trigger all effect tiles that are set to trigger at the specified time.
        /// </summary>
        /// <param name="effectTriggerTime"></param>
        /// <returns>True if any tile can trigger this phase.</returns>
        public static bool TriggerEffectTiles(EffectTriggerTime effectTriggerTime)
        {
            List<IEffectTile> effectTiles = MapContainer.GameGrid[(int) Layer.Entities].OfType<IEffectTile>().ToList();
            List<IEffectTile> triggerTiles = effectTiles.Where(tile => tile.WillTrigger(effectTriggerTime)).ToList();

            if (triggerTiles.Count <= 0)
            {
                effectTiles.ForEach(tile => tile.HasTriggered = false);
                return false;
            }

            Queue<IEvent> effectTileEvents = new Queue<IEvent>();
            effectTileEvents.Enqueue(new WaitFramesEvent(10));
            effectTileEvents.Enqueue(
                new ToastAtCursorEvent(
                    "Resolving " + effectTriggerTime + " Tile Effects...",
                    AssetManager.MenuConfirmSFX,
                    100
                )
            );
            effectTileEvents.Enqueue(new WaitFramesEvent(50));

            foreach (IEffectTile tile in triggerTiles)
            {
                effectTileEvents.Enqueue(new CameraCursorPositionEvent(tile.MapCoordinates));
                effectTileEvents.Enqueue(
                    new TriggerSingleEffectTileEvent(tile, effectTriggerTime, 60)
                );
            }

            effectTileEvents.Enqueue(new RemoveExpiredEffectTilesEvent(triggerTiles));

            switch (effectTriggerTime)
            {
                case EffectTriggerTime.StartOfRound:
                    effectTileEvents.Enqueue(new EffectTilesStartOfRoundEvent());
                    break;
                case EffectTriggerTime.EndOfTurn:
                    effectTileEvents.Enqueue(new EndTurnEvent());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(effectTriggerTime), effectTriggerTime, null);
            }

            GlobalEventQueue.QueueEvents(effectTileEvents);

            return true;
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

        public void ToggleActionInventoryMenu()
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
            if (GameMapView.CurrentMenu.CurrentOption is ActionOption actionOption)
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

            switch (currentSlice.ItemEntity)
            {
                case Spoils spoils:
                    items.AddRange(spoils.Items);
                    break;
                case IItem item:
                    items.Add(item);
                    break;
            }

            if (currentSlice.TerrainEntity is Vendor vendor)
            {
                items.AddRange(vendor.Items);
            }

            GameUnit sliceUnit = UnitSelector.SelectUnit(currentSlice.UnitEntity);
            sliceUnit?.Inventory.ForEach(item => items.Add(item));

            return items;
        }

        private static bool ShowingItemPreview => GameMapView.ItemDetailWindow != null;
    }
}
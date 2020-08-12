using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NLog;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;
using SolStandard.Utility.Inputs;

namespace SolStandard.Containers.Components.World
{
    public class WorldContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public enum TurnState
        {
            SelectUnit,
            UnitMoving,
            UnitDecidingAction,
            UnitTargeting,
            UnitActing,
            FinishingCombat,
            ResolvingTurn,
            AdHocDraft,
            TakeItem
        }

        private TurnState currentTurnState;

        public TurnState CurrentTurnState
        {
            get => currentTurnState;
            set
            {
                Logger.Debug("Changing game turn state: {}", value);
                currentTurnState = value;
            }
        }

        public GameUnit SelectedUnit { get; private set; }
        private Vector2 selectedUnitOriginalPosition;
        public static WorldHUD WorldHUD { get; private set; }
        public MapContainer MapContainer { get; }
        private int TurnCounter { get; set; }
        private int RoundCounter { get; set; }
        private bool CanCancelAction { get; set; }
        private GameUnit HoverUnit { get; set; }
        private static bool ShowingItemPreview => WorldHUD.ItemDetailWindow != null;

        private readonly Dictionary<Direction, UnitAnimationState> directionToAnimation =
            new Dictionary<Direction, UnitAnimationState>
            {
                {Direction.Down, UnitAnimationState.WalkDown},
                {Direction.Up, UnitAnimationState.WalkUp},
                {Direction.Right, UnitAnimationState.WalkRight},
                {Direction.Left, UnitAnimationState.WalkLeft}
            };

        public WorldContext(MapContainer mapContainer, WorldHUD gameMapController)
        {
            MapContainer = mapContainer;
            WorldHUD = gameMapController;
            CurrentTurnState = TurnState.SelectUnit;
            selectedUnitOriginalPosition = new Vector2();
            TurnCounter = 1;
            RoundCounter = 1;
            CanCancelAction = true;
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public bool CanPressConfirm => CurrentTurnState != TurnState.SelectUnit ||
                                       HoverUnit != null && HoverUnit.Team == GlobalContext.ActiveTeam;

        public bool CanPressCancel
        {
            get
            {
                return CurrentTurnState switch
                {
                    TurnState.UnitMoving => true,
                    TurnState.UnitDecidingAction => CanCancelAction,
                    TurnState.UnitTargeting => true,
                    TurnState.TakeItem => true,
                    _ => false
                };
            }
        }

        public bool CanPressPreviewItem
        {
            get
            {
                if (CurrentTurnState != TurnState.SelectUnit) return false;

                MapSlice cursorSlice = MapContainer.GetMapSliceAtCursor();
                switch (cursorSlice.TerrainEntity)
                {
                    case Vendor hoverVendor:
                        return hoverVendor.Items.Count(x => x != null) > 0;
                    case Chest hoverChest:
                        return !string.IsNullOrEmpty(hoverChest.ItemPool);
                }

                if (cursorSlice.ItemEntity != null && !(cursorSlice.ItemEntity is Currency)) return true;

                return HoverUnit != null && HoverUnit.Inventory.Count > 0;
            }
        }

        public bool CanPressPreviewUnit
        {
            get
            {
                if (CurrentTurnState != TurnState.SelectUnit) return false;
                return HoverUnit != null;
            }
        }


        public void PlayAnimationAtCoordinates(TriggeredAnimation animation, Vector2 coordinates)
        {
            animation.PlayOnce();
            MapContainer.GameGrid[(int) Layer.OverlayEffect][(int) coordinates.X, (int) coordinates.Y] =
                new Decoration("Interaction", "Decoration", animation, coordinates);
        }

        public static void UpdateWindowsEachTurn()
        {
            WorldHUD.GenerateInitiativeWindow();
            WorldHUD.GenerateObjectiveWindow();
        }

        public static void FinishTurn(bool skipProcs)
        {
            MapContainer.ClearDynamicAndPreviewGrids();

            if (GlobalContext.WorldContext.SelectedUnit != null)
            {
                GlobalContext.WorldContext.SelectedUnit.SetUnitAnimation(UnitAnimationState.Idle);

                if (!skipProcs)
                {
                    Logger.Trace("Resolving turn procs.");

                    IEnumerable<ITurnProc> activeUnitTurnProcs = GlobalContext.ActiveUnit.StatusEffects
                        .Where(effect => effect is ITurnProc)
                        .Cast<ITurnProc>();

                    foreach (ITurnProc turnProc in activeUnitTurnProcs)
                    {
                        turnProc.OnTurnEnd();
                    }
                }
            }

            SetPromptWindowText("Confirm End Turn");
            GlobalContext.WorldContext.CurrentTurnState = TurnState.ResolvingTurn;
        }

        public void ResolveTurn()
        {
            if (GlobalContext.CurrentGameState == GlobalContext.GameState.Results) return;

            Logger.Trace("Resolving turn.");

            GlobalContext.Scenario.CheckForWinState();
            ConfirmPromptWindow();
            GlobalContext.InitiativePhase.PassTurnToNextUnit();
            UpdateWindowsEachTurn();

            ResetTurnState();
            UpdateTurnCounters();

            if (NotEveryUnitIsDead())
            {
                EndTurnIfUnitIsDead();
            }

            GlobalContext.StatusScreenHUD.UpdateWindows();

            StartTurn();
            ResetCursorToActiveUnit();
        }

        private void StartTurn()
        {
            CanCancelAction = true;

            if (GlobalContext.WorldContext.SelectedUnit == null) return;

            IEnumerable<ITurnProc> activeUnitTurnProcs = GlobalContext.ActiveUnit.StatusEffects
                .Where(effect => effect is ITurnProc)
                .Cast<ITurnProc>();

            foreach (ITurnProc turnProc in activeUnitTurnProcs)
            {
                turnProc.OnTurnStart();
            }
        }

        private static bool NotEveryUnitIsDead()
        {
            return !GlobalContext.Units.TrueForAll(unit => unit.Stats.CurrentHP <= 0);
        }

        private void UpdateTurnCounters()
        {
            TurnCounter++;

            if (TurnCounter < GlobalContext.Units.Count) return;

            TurnCounter = 1;
            RoundCounter++;
        }


        private void EndTurnIfUnitIsDead()
        {
            if (CurrentTurnState == TurnState.SelectUnit && GlobalContext.ActiveUnit.UnitEntity == null)
            {
                ResolveTurn();
            }
        }

        private void RevertToPreviousState()
        {
            if (CurrentTurnState <= TurnState.SelectUnit) return;

            CurrentTurnState--;
            Logger.Debug("Changing state: " + CurrentTurnState);
        }

        public void ResetTurnState()
        {
            CurrentTurnState = TurnState.SelectUnit;
            Logger.Debug("Resetting to initial state: " + CurrentTurnState);
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
            WorldHUD.GenerateActionMenus();
            WorldHUD.GenerateCurrentMenuDescription();

            GenerateActionPreviewGrid();
        }

        public void ResetCursorToNextUnitOnTeam()
        {
            GlobalContext.InitiativePhase.SelectNextUnitOnActiveTeam();

            if (GlobalContext.ActiveUnit.UnitEntity == null) return;

            MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates);
            MapContainer.MapCamera.CenterCameraToCursor();
        }

        public void ResetCursorToPreviousUnitOnTeam()
        {
            GlobalContext.InitiativePhase.SelectPreviousUnitOnActiveTeam();

            if (GlobalContext.ActiveUnit.UnitEntity == null) return;

            MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates);
            MapContainer.MapCamera.CenterCameraToCursor();
        }


        public void ResetCursorToActiveUnit()
        {
            if (GlobalContext.ActiveUnit.UnitEntity == null) return;

            MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates);
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

                if (WorldHUD.ActionMenuContext.IsAtRootMenu)
                {
                    CancelActionMenuAndReturnToOrigin();
                }
                else
                {
                    WorldHUD.ActionMenuContext.GoToPreviousMenu();
                    WorldHUD.GenerateCurrentMenuDescription();
                    AssetManager.MapUnitCancelSFX.Play();
                }
            }
            else
            {
                CancelExtraAction();
            }
        }

        private void CancelActionMenuAndReturnToOrigin()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            WorldHUD.CloseCombatMenu();
            RevertToPreviousState();
            CancelMove();
        }

        public void CancelUnitTargeting()
        {
            if (CanCancelAction)
            {
                CancelTargetAndOpenLastActionMenu();
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
                CancelTargetAndOpenLastActionMenu();
            }
            else
            {
                if (WorldHUD.ActionMenuContext.IsAtRootMenu)
                {
                    MapContainer.AddNewToastAtMapCursor("Can't cancel action!", 50);

                    AssetManager.WarningSFX.Play();
                }
                else
                {
                    WorldHUD.ActionMenuContext.GoToPreviousMenu();
                    AssetManager.MapUnitCancelSFX.Play();
                }
            }
        }

        private void CancelTargetAndOpenLastActionMenu()
        {
            GlobalContext.ActiveUnit.CancelArmedSkill();
            ResetCursorToActiveUnit();
            WorldHUD.ActionMenuContext.Unhide();
            RevertToPreviousState();
            AssetManager.MapUnitCancelSFX.Play();
        }

        private void StartMoving()
        {
            if (SelectedUnit != null)
            {
                Logger.Debug("Selecting unit: " + SelectedUnit.Team + " " + SelectedUnit.Role);
                CurrentTurnState = TurnState.UnitMoving;
                GenerateMoveGrid(
                    MapContainer.MapCursor.MapCoordinates,
                    SelectedUnit,
                    MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement)
                );

                MapContainer.ClearPreviewGrid();
                new UnitTargetingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateTargetingGrid(
                        SelectedUnit.UnitEntity.MapCoordinates,
                        SelectedUnit.Stats.CurrentAtkRange,
                        Layer.Preview
                    );
            }
            else
            {
                Logger.Debug("No unit to select.");
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

            if (GlobalContext.InitiativePhase.SelectActiveUnit(SelectedUnit)) return true;
            //If the entity selected isn't the active unit, don't select it.
            SelectedUnit = null;
            AssetManager.WarningSFX.Play();
            MapContainer.AddNewToastAtMapCursor("Not an active unit!", 50);
            return false;
        }

        public void ExecuteAction()
        {
            GlobalContext.ActiveUnit.ExecuteArmedSkill(MapContainer.GetMapSliceAtCursor());
        }


        public static void SetPromptWindowText(string promptText)
        {
            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(AssetManager.PromptFont, promptText),
                    InputIconProvider.GetInputIcon(Input.Confirm,
                        Convert.ToInt32(AssetManager.PromptFont.MeasureString("A").Y))
                }
            };
            var promptWindowContentGrid = new WindowContentGrid(promptTextContent, 2);
            WorldHUD.GenerateUserPromptWindow(promptWindowContentGrid, new Vector2(0, 150));
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
            WorldHUD.CloseUserPromptWindow();
        }

        private void GenerateMoveGrid(Vector2 origin, GameUnit selectedUnit, SpriteAtlas spriteAtlas)
        {
            selectedUnitOriginalPosition = origin;
            var unitMovingContext = new UnitMovingPhase(spriteAtlas);
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
            new UnitTargetingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                .GenerateTargetingGrid(SelectedUnit.UnitEntity.MapCoordinates, SelectedUnit.Stats.CurrentAtkRange,
                    Layer.Preview);
        }

        public void UpdateHoverContextWindows()
        {
            MapSlice hoverSlice = MapContainer.GetMapSliceAtCursor();

            GameUnit hoverMapUnit = UnitSelector.SelectUnit(hoverSlice.UnitEntity);

            if (CurrentTurnState != TurnState.SelectUnit)
            {
                if (hoverMapUnit != GlobalContext.ActiveUnit)
                {
                    WorldHUD.UpdateLeftPortraitAndDetailWindows(GlobalContext.ActiveUnit);
                    WorldHUD.UpdateRightPortraitAndDetailWindows(hoverMapUnit);
                }
                else
                {
                    WorldHUD.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                    WorldHUD.UpdateRightPortraitAndDetailWindows(null);
                }
            }
            else
            {
                UpdateThreatRangePreview(hoverMapUnit, hoverSlice);

                WorldHUD.UpdateLeftPortraitAndDetailWindows(hoverMapUnit);
                WorldHUD.UpdateRightPortraitAndDetailWindows(null);
            }

            //Terrain (Entity) Window
            WorldHUD.SetEntityWindow(hoverSlice);

            HoverUnit = hoverMapUnit;
        }

        private void UpdateThreatRangePreview(GameUnit hoverMapUnit, MapSlice hoverSlice)
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            if (hoverMapUnit != null && GlobalContext.ActiveTeam != Team.Creep)
            {
                if (MapContainer.GetMapElementsFromLayer(Layer.Dynamic).Count != 0 && HoverUnit == hoverMapUnit) return;

                new UnitTargetingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateThreatGrid(hoverSlice.MapCoordinates, hoverMapUnit, hoverMapUnit.Team);
            }
            else if (hoverSlice.TerrainEntity is IThreatRange entityThreat)
            {
                new UnitTargetingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateThreatGrid(hoverSlice.MapCoordinates, entityThreat);
            }
            else if (hoverSlice.TerrainEntity is IActionTile actionTile)
            {
                new UnitTargetingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action))
                    .GenerateTargetingGrid(hoverSlice.MapCoordinates, actionTile.InteractRange, Layer.Preview);
            }
            else if (hoverSlice.ItemEntity is IActionTile itemActionTile)
            {
                new UnitTargetingPhase(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action))
                    .GenerateTargetingGrid(hoverSlice.MapCoordinates, itemActionTile.InteractRange, Layer.Preview);
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
            MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(selectedUnitOriginalPosition);
        }

        public void MoveActionMenuCursor(MenuCursorDirection direction)
        {
            WorldHUD.CurrentMenu.MoveMenuCursor(direction);
            WorldHUD.GenerateCurrentMenuDescription();

            GenerateActionPreviewGrid();
        }

        public void SelectActionMenuOption()
        {
            AssetManager.MapUnitSelectSFX.Play();
            MapContainer.ClearDynamicAndPreviewGrids();

            //FIXME This part of the code shouldn't know so much about the menu?
            if (WorldHUD?.CurrentMenu?.CurrentOption is ActionOption)
            {
                WorldHUD.CurrentMenu.CurrentOption.Execute();
                WorldHUD.ActionMenuContext.Hide();
                CurrentTurnState = TurnState.UnitTargeting;
                SelectedUnit.SetUnitAnimation(UnitAnimationState.Active);
            }
            else
            {
                WorldHUD?.CurrentMenu?.CurrentOption.Execute();
                WorldHUD?.GenerateCurrentMenuDescription();
                GenerateActionPreviewGrid();
            }
        }

        public void RefreshCurrentActionMenuOption()
        {
            WorldHUD.CurrentMenu.CurrentOption.Refresh();
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
            if (!(WorldHUD.CurrentMenu.CurrentOption is ActionOption currentActionOption)) return null;
            return currentActionOption.Action as IIncrementableAction;
        }

        /// <summary>
        /// Trigger all effect tiles that are set to trigger at the specified time.
        /// </summary>
        /// <param name="effectTriggerTime"></param>
        /// <param name="isCreepEvent"></param>
        /// <returns>True if any tile can trigger this phase.</returns>
        public static bool TriggerEffectTiles(EffectTriggerTime effectTriggerTime, bool isCreepEvent)
        {
            List<IEffectTile> effectTiles = MapContainer.GameGrid[(int) Layer.Entities].OfType<IEffectTile>().ToList();
            List<IEffectTile> triggerTiles = effectTiles.Where(tile => tile.WillTrigger(effectTriggerTime)).ToList();

            if (triggerTiles.Count <= 0)
            {
                effectTiles.ForEach(tile => tile.HasTriggered = false);
                return false;
            }

            var effectTileEvents = new Queue<IEvent>();
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
                    new TriggerSingleEffectTileEvent(tile, effectTriggerTime, 80)
                );
            }

            effectTileEvents.Enqueue(new RemoveExpiredEffectTilesEvent(triggerTiles));

            switch (effectTriggerTime)
            {
                case EffectTriggerTime.StartOfRound:
                    effectTileEvents.Enqueue(new EffectTilesStartOfRoundEvent());
                    break;
                case EffectTriggerTime.EndOfTurn:
                    if (isCreepEvent)
                    {
                        effectTileEvents.Enqueue(new CreepEndTurnEvent());
                    }
                    else
                    {
                        effectTileEvents.Enqueue(new EndTurnEvent());
                    }

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

        public void OpenTakeItemMenu(GameUnit targetToTakeFrom, bool freeAction)
        {
            CurrentTurnState = TurnState.TakeItem;
            WorldHUD.GenerateTakeItemMenu(targetToTakeFrom, freeAction);
            AssetManager.MenuConfirmSFX.Play();
        }

        public void MoveMenuCursor(MenuCursorDirection direction)
        {
            WorldHUD.CurrentMenu.MoveMenuCursor(direction);
        }

        public void SelectMenuOption()
        {
            WorldHUD.CurrentMenu.SelectOption();
        }

        public void ClearStealItemMenu()
        {
            WorldHUD.CloseStealItemMenu();
            MapContainer.ClearDynamicAndPreviewGrids();
        }

        public void CancelStealItemMenu()
        {
            WorldHUD.CloseStealItemMenu();
            GlobalContext.ActiveUnit.CancelArmedSkill();
            ResetCursorToActiveUnit();
            WorldHUD.GenerateActionMenus();
            CurrentTurnState = TurnState.UnitDecidingAction;
        }

        public void OpenDraftMenu()
        {
            CurrentTurnState = TurnState.AdHocDraft;
            WorldHUD.GenerateDraftMenu(GlobalContext.ActiveTeam);
            AssetManager.MenuConfirmSFX.Play();
        }

        public void ClearDraftMenu()
        {
            WorldHUD.CloseAdHocDraftMenu();
            MapContainer.ClearDynamicAndPreviewGrids();
        }

        private static void GenerateActionPreviewGrid()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            if (WorldHUD.CurrentMenu.CurrentOption is ActionOption actionOption)
            {
                actionOption.Action.GenerateActionGrid(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates,
                    Layer.Preview);
            }
        }

        public void ShowUnitCodexEntry()
        {
            MapSlice currentSlice = MapContainer.GetMapSliceAtCursor();

            GameUnit selectedUnit = UnitSelector.SelectUnit(currentSlice.UnitEntity);

            if (selectedUnit == null) return;

            GlobalContext.CodexContext.OpenMenu();
            GlobalContext.CodexContext.ShowUnitDetails(selectedUnit);
            AssetManager.MapUnitSelectSFX.Play();
        }

        public void ToggleItemPreview()
        {
            if (ShowingItemPreview)
            {
                AssetManager.MapUnitCancelSFX.Play();
                WorldHUD.CloseItemDetailWindow();
                GlobalContext.CurrentGameState = GlobalContext.GameState.InGame;
            }
            else
            {
                MapSlice currentSlice = MapContainer.GetMapSliceAtCursor();

                List<IItem> items = CollectItemsFromSlice(currentSlice);

                if (items.Count > 0)
                {
                    AssetManager.MapUnitSelectSFX.Play();
                    WorldHUD.GenerateItemDetailWindow(items);
                    GlobalContext.CurrentGameState = GlobalContext.GameState.ItemPreview;
                }
                else
                {
                    AssetManager.WarningSFX.Play();
                    MapContainer.AddNewToastAtMapCursor("No items to preview!", 50);
                }
            }
        }

        public static List<IItem> CollectItemsFromSlice(MapSlice currentSlice)
        {
            var items = new List<IItem>();

            switch (currentSlice.ItemEntity)
            {
                case Spoils spoils:
                    items.AddRange(spoils.Items);
                    break;
                case IItem item:
                    items.Add(item);
                    break;
            }

            switch (currentSlice.TerrainEntity)
            {
                case Vendor vendor:
                    items.AddRange(vendor.Items);
                    break;
                case Chest chest:
                    items.AddRange(GlobalContext.WorldContext.MapContainer.GetPoolItems(chest.ItemPool));
                    break;
            }

            GameUnit sliceUnit = UnitSelector.SelectUnit(currentSlice.UnitEntity);
            sliceUnit?.Inventory.ForEach(item => items.Add(item));

            return items;
        }
    }
}
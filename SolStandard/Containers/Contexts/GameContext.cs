using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;
using TiledSharp;

namespace SolStandard.Containers.Contexts
{
    public class GameContext
    {
        public enum GameState
        {
            MainMenu,
            ModeSelect,
            ArmyDraft,
            MapSelect,
            PauseScreen,
            InGame,
            Results
        }

        public static readonly Color PositiveColor = new Color(30, 200, 30);
        public static readonly Color NegativeColor = new Color(250, 10, 10);
        public static readonly Color NeutralColor = new Color(255, 250, 250);

        private readonly BattleContext battleContext;
        public static MapSelectContext MapSelectContext { get; private set; }
        public MapContext MapContext { get; private set; }
        public ResultsUI ResultsUI { get; private set; }
        public MainMenuUI MainMenuUI { get; private set; }
        public static int TurnCounter { get; private set; }
        public static int RoundCounter { get; private set; }
        private float oldZoom;

        public static GameState CurrentGameState;
        public static PlayerIndex ActivePlayer { get; set; }

        private static InitiativeContext InitiativeContext { get; set; }


        public static List<GameUnit> Units
        {
            get { return InitiativeContext.InitiativeList; }
        }

        public static GameUnit ActiveUnit
        {
            get { return InitiativeContext.CurrentActiveUnit; }
        }

        public BattleContext BattleContext
        {
            get { return battleContext; }
        }

        public GameContext(MainMenuUI mainMenuUI)
        {
            battleContext = new BattleContext(new BattleUI());
            MainMenuUI = mainMenuUI;

            LoadMapSelect();

            CurrentGameState = GameState.MainMenu;
        }

        public static void LoadMapSelect()
        {
            const string mapFile = "Map_Select_02.tmx";
            const string mapSelectPath = "Content/TmxMaps/" + mapFile;

            TmxMapParser mapParser = new TmxMapParser(
                new TmxMap(mapSelectPath),
                AssetManager.OverworldTexture,
                AssetManager.EntitiesTexture,
                AssetManager.UnitSprites,
                GameDriver.TmxObjectTypeDefaults);

            MapSelectContext = new MapSelectContext(new SelectMapUI(),
                new MapContainer(mapParser.LoadMapGrid(), AssetManager.MapCursorTexture));

            LoadInitiativeContext(mapParser);

            CurrentGameState = GameState.MapSelect;
        }

        public void StartGame(string mapPath)
        {
            LoadMap(mapPath);

            TurnCounter = 1;
            RoundCounter = 1;

            foreach (GameUnit unit in Units)
            {
                unit.DisableExhaustedUnit();
            }

            ActiveUnit.ActivateUnit();
            MapContext.SnapCursorToActiveUnit();
            MapCamera.SnapCameraCenterToCursor();
            MapContext.EndTurn();

            MapContext.UpdateWindowsEachTurn();
            ResultsUI.UpdateWindows();

            CurrentGameState = GameState.InGame;
        }

        private void LoadMap(string mapPath)
        {
            TmxMapParser mapParser = new TmxMapParser(
                new TmxMap(mapPath),
                AssetManager.OverworldTexture,
                AssetManager.EntitiesTexture,
                AssetManager.UnitSprites,
                GameDriver.TmxObjectTypeDefaults
            );

            LoadMapContainer(mapParser);
            LoadInitiativeContext(mapParser);
            LoadResultsUI();
        }

        private void LoadResultsUI()
        {
            ResultsUI = new ResultsUI();
        }

        private void LoadMapContainer(TmxMapParser mapParser)
        {
            ITexture2D mapCursorTexture = AssetManager.MapCursorTexture;

            MapContext = new MapContext(
                new MapContainer(mapParser.LoadMapGrid(), mapCursorTexture),
                new GameMapUI(GameDriver.ScreenSize)
            );
        }

        private static void LoadInitiativeContext(TmxMapParser mapParser)
        {
            List<GameUnit> unitsFromMap = UnitGenerator.GenerateUnitsFromMap(
                mapParser.LoadUnits(),
                AssetManager.LargePortraitTextures,
                AssetManager.MediumPortraitTextures,
                AssetManager.SmallPortraitTextures
            );

            //Randomize the team that goes first
            InitiativeContext =
                new InitiativeContext(unitsFromMap, (GameDriver.Random.Next(2) == 0) ? Team.Blue : Team.Red);
        }


        public void SelectUnitAndStartMoving()
        {
            if (!TrySelectUnit()) return;
            StartMoving();
            AssetManager.MapUnitSelectSFX.Play();
        }

        public void FinishMoving()
        {
            if (MapContext.OtherUnitExistsAtCursor() || MapContainer.GetMapSliceAtCursor().DynamicEntity == null)
            {
                MapContainer.AddNewToastAtMapCursor("Cannot end move on this space!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            MapContext.ProceedToNextState();

            MapContainer.ClearDynamicAndPreviewGrids();
            MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
            AssetManager.MapUnitSelectSFX.Play();

            MapContext.GameMapUI.GenerateActionMenu();
            MapContext.GenerateActionPreviewGrid();
        }

        public void DecideAction()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            MapContext.GameMapUI.ActionMenu.CurrentOption.Execute();
            MapContext.GameMapUI.ClearCombatMenu();

            MapContext.ProceedToNextState();
            MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            AssetManager.MapUnitSelectSFX.Play();
        }

        public void ExecuteAction()
        {
            ActiveUnit.ExecuteArmedSkill(MapContainer.GetMapSliceAtCursor(), MapContext, battleContext);
        }

        public void CancelAction()
        {
            ActiveUnit.CancelArmedSkill(MapContext);
            MapContext.SlideCursorToActiveUnit();
            MapContext.GameMapUI.GenerateActionMenu();
        }

        public void ContinueCombat()
        {
            switch (BattleContext.CurrentState)
            {
                case BattleContext.BattleState.Start:
                    if (BattleContext.TryProceedToNextState())
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        BattleContext.StartRollingDice();
                    }

                    break;
                case BattleContext.BattleState.RollDice:
                    if (BattleContext.TryProceedToNextState())
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        BattleContext.StartResolvingBlocks();
                    }

                    break;
                case BattleContext.BattleState.CountDice:
                    if (BattleContext.TryProceedToNextState())
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        BattleContext.StartResolvingDamage();
                    }

                    break;
                case BattleContext.BattleState.ResolveCombat:
                    if (BattleContext.TryProceedToNextState())
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        MapContext.ProceedToNextState();
                    }

                    break;
                default:
                    MapContext.ProceedToNextState();
                    return;
            }
        }

        public void UpdateCamera(MapCamera mapCamera)
        {
            if (CurrentGameState != GameState.InGame) return;

            switch (MapContext.CurrentTurnState)
            {
                case MapContext.TurnState.SelectUnit:
                    break;
                case MapContext.TurnState.UnitMoving:
                    break;
                case MapContext.TurnState.UnitDecidingAction:
                    break;
                case MapContext.TurnState.UnitTargeting:
                    oldZoom = MapCamera.CurrentZoom;
                    break;
                case MapContext.TurnState.UnitActing:
                    const float combatZoom = 4;
                    mapCamera.ZoomToCursor(combatZoom);
                    break;
                case MapContext.TurnState.ResolvingTurn:
                    mapCamera.ZoomToCursor(oldZoom);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ResolveTurn()
        {
            GameScenario.CheckForWinState(this);

            MapContext.ConfirmPromptWindow();
            ActiveUnit.DisableExhaustedUnit();
            InitiativeContext.PassTurnToNextUnit();
            ActiveUnit.ActivateUnit();
            MapContext.SnapCursorToActiveUnit();
            MapCamera.CenterCameraToCursor();

            MapContext.EndTurn();

            UpdateTurnCounters();

            if (!Units.TrueForAll(unit => unit.Stats.Hp <= 0))
            {
                EndTurnIfUnitIsDead();
            }

            MapContext.UpdateWindowsEachTurn();
            ResultsUI.UpdateWindows();

            AssetManager.MapUnitSelectSFX.Play();
        }

        private static void UpdateTurnCounters()
        {
            TurnCounter++;

            if (TurnCounter <= Units.Count) return;

            TurnCounter = 1;
            RoundCounter++;
        }


        public void CancelMove()
        {
            MapContext.CancelMovement();
            AssetManager.MapUnitCancelSFX.Play();
        }

        private void EndTurnIfUnitIsDead()
        {
            if (MapContext.CurrentTurnState == MapContext.TurnState.SelectUnit && ActiveUnit.UnitEntity == null)
            {
                ResolveTurn();
            }
        }

        private void StartMoving()
        {
            if (MapContext.SelectedUnit != null)
            {
                Trace.WriteLine("Selecting unit: " + MapContext.SelectedUnit.Team + " " + MapContext.SelectedUnit.Role);
                MapContext.ProceedToNextState();
                MapContext.GenerateMoveGrid(
                    MapContainer.MapCursor.MapCoordinates,
                    MapContext.SelectedUnit,
                    new SpriteAtlas(
                        new Texture2DWrapper(AssetManager.ActionTiles.MonoGameTexture),
                        new Vector2(GameDriver.CellSize),
                        (int) MapDistanceTile.TileType.Movement
                    )
                );

                MapContainer.ClearPreviewGrid();
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack))
                    .GenerateTargetingGrid(
                        MapContext.SelectedUnit.UnitEntity.MapCoordinates,
                        MapContext.SelectedUnit.Stats.AtkRange,
                        Layer.Preview
                    );
            }
            else
            {
                Trace.WriteLine("No unit to select.");
            }
        }

        private bool TrySelectUnit()
        {
            //Select the unit. Store it somewhere.
            MapContext.SelectedUnit = UnitSelector.SelectUnit(MapContainer.GetMapSliceAtCursor().UnitEntity);

            //If the entity selected isn't the active unit, don't select it.
            if (MapContext.SelectedUnit != ActiveUnit)
            {
                MapContext.SelectedUnit = null;
                //TODO Notify the player the selected unit is not legal
                return false;
            }

            return true;
        }
    }
}
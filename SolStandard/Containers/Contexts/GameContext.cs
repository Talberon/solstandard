﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts.WinConditions;
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
        public static Scenario Scenario { get; private set; }
        public static MapSelectContext MapSelectContext { get; private set; }
        public GameMapContext GameMapContext { get; private set; }
        public StatusUI StatusUI { get; private set; }
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
            oldZoom = MapCamera.CurrentZoom;
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

        //TODO Read the different available scenarios from the props of the Map that gets selected on the select screen
        private static Dictionary<VictoryConditions, Objective> DefaultScenarios
        {
            get
            {
                return new Dictionary<VictoryConditions, Objective>
                {
                    {VictoryConditions.Surrender, new Surrender()},
                    {VictoryConditions.DefeatCommander, new DefeatCommander()},
                    {VictoryConditions.Taxes, new Taxes(100)}
                };
            }
        }

        public void StartGame(string mapPath)
        {
            Scenario = new Scenario(DefaultScenarios);

            LoadMap(mapPath);

            TurnCounter = 1;
            RoundCounter = 1;

            foreach (GameUnit unit in Units)
            {
                unit.DisableExhaustedUnit();
            }

            ActiveUnit.ActivateUnit();
            GameMapContext.ResetCursorToActiveUnit();
            MapCamera.SnapCameraCenterToCursor();
            GameMapContext.EndTurn();

            GameMapContext.UpdateWindowsEachTurn();
            StatusUI.UpdateWindows();

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
            StatusUI = new StatusUI();
        }

        private void LoadMapContainer(TmxMapParser mapParser)
        {
            ITexture2D mapCursorTexture = AssetManager.MapCursorTexture;

            GameMapContext = new GameMapContext(
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
            if (GameMapContext.OtherUnitExistsAtCursor() || MapContainer.GetMapSliceAtCursor().DynamicEntity == null)
            {
                MapContainer.AddNewToastAtMapCursor("Cannot end move on this space!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            GameMapContext.ProceedToNextState();

            MapContainer.ClearDynamicAndPreviewGrids();
            GameMapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
            AssetManager.MapUnitSelectSFX.Play();

            GameMapContext.GameMapUI.GenerateActionMenu();
            GameMapContext.GenerateActionPreviewGrid();
        }

        public void ExecuteAction()
        {
            ActiveUnit.ExecuteArmedSkill(MapContainer.GetMapSliceAtCursor(), GameMapContext, battleContext);
        }

        public void CancelAction()
        {
            ActiveUnit.CancelArmedSkill(GameMapContext);
            GameMapContext.ResetCursorToActiveUnit();
            GameMapContext.GameMapUI.GenerateActionMenu();
        }

        public void ContinueCombat()
        {
            switch (BattleContext.CurrentState)
            {
                case BattleContext.BattleState.Start:
                    if (BattleContext.TryProceedToState(BattleContext.BattleState.RollDice))
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        BattleContext.StartRollingDice();
                    }

                    break;
                case BattleContext.BattleState.RollDice:
                    if (BattleContext.TryProceedToState(BattleContext.BattleState.ResolveCombat))
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        BattleContext.StartResolvingBlocks();
                    }

                    break;
                case BattleContext.BattleState.ResolveCombat:
                    if (BattleContext.TryProceedToState(BattleContext.BattleState.Start))
                    {
                        AssetManager.MapUnitSelectSFX.Play();
                        GameMapContext.ProceedToNextState();
                    }

                    break;
                default:
                    GameMapContext.ProceedToNextState();
                    return;
            }
        }

        public void UpdateCamera(MapCamera mapCamera)
        {
            if (CurrentGameState != GameState.InGame) return;

            switch (GameMapContext.CurrentTurnState)
            {
                case GameMapContext.TurnState.SelectUnit:
                    break;
                case GameMapContext.TurnState.UnitMoving:
                    break;
                case GameMapContext.TurnState.UnitDecidingAction:
                    break;
                case GameMapContext.TurnState.UnitTargeting:
                    oldZoom = MapCamera.CurrentZoom;
                    break;
                case GameMapContext.TurnState.UnitActing:
                    const float combatZoom = 4;
                    mapCamera.ZoomToCursor(combatZoom);
                    break;
                case GameMapContext.TurnState.ResolvingTurn:
                    mapCamera.ZoomToCursor(oldZoom);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ResolveTurn()
        {
            Scenario.CheckForWinState(this);

            GameMapContext.ConfirmPromptWindow();
            ActiveUnit.DisableExhaustedUnit();
            InitiativeContext.PassTurnToNextUnit();
            ActiveUnit.ActivateUnit();
            GameMapContext.UpdateWindowsEachTurn();
            GameMapContext.ResetCursorToActiveUnit();
            MapCamera.CenterCameraToCursor();

            GameMapContext.EndTurn();

            UpdateTurnCounters();

            if (!Units.TrueForAll(unit => unit.Stats.Hp <= 0))
            {
                EndTurnIfUnitIsDead();
            }

            StatusUI.UpdateWindows();

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
            GameMapContext.CancelMovement();
            AssetManager.MapUnitCancelSFX.Play();
        }

        private void EndTurnIfUnitIsDead()
        {
            if (GameMapContext.CurrentTurnState == GameMapContext.TurnState.SelectUnit && ActiveUnit.UnitEntity == null)
            {
                ResolveTurn();
            }
        }

        private void StartMoving()
        {
            if (GameMapContext.SelectedUnit != null)
            {
                Trace.WriteLine("Selecting unit: " + GameMapContext.SelectedUnit.Team + " " +
                                GameMapContext.SelectedUnit.Role);
                GameMapContext.ProceedToNextState();
                GameMapContext.GenerateMoveGrid(
                    MapContainer.MapCursor.MapCoordinates,
                    GameMapContext.SelectedUnit,
                    new SpriteAtlas(
                        new Texture2DWrapper(AssetManager.ActionTiles.MonoGameTexture),
                        new Vector2(GameDriver.CellSize),
                        (int) MapDistanceTile.TileType.Movement
                    )
                );

                MapContainer.ClearPreviewGrid();
                new UnitTargetingContext(MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack), false)
                    .GenerateTargetingGrid(
                        GameMapContext.SelectedUnit.UnitEntity.MapCoordinates,
                        GameMapContext.SelectedUnit.Stats.AtkRange,
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
            GameMapContext.SelectedUnit = UnitSelector.SelectUnit(MapContainer.GetMapSliceAtCursor().UnitEntity);

            //If the entity selected isn't the active unit, don't select it.
            if (GameMapContext.SelectedUnit != ActiveUnit)
            {
                GameMapContext.SelectedUnit = null;
                //TODO Notify the player the selected unit is not legal
                return false;
            }

            return true;
        }
    }
}
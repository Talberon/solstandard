using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Utility;
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

        private MapContext mapContext;
        private readonly BattleContext battleContext;
        public ResultsUI ResultsUI { get; private set; }
        public MainMenuUI MainMenuUI { get; private set; }
        public MapSelectionMenuUI MapSelectionMenuUI { get; private set; }
        public static int TurnNumber { get; private set; }
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

        public MapContext MapContext
        {
            get { return mapContext; }
        }

        public BattleContext BattleContext
        {
            get { return battleContext; }
        }

        public GameContext(MainMenuUI mainMenuUI, MapSelectionMenuUI mapSelectionMenuUI)
        {
            battleContext = new BattleContext(new BattleUI());
            MainMenuUI = mainMenuUI;
            MapSelectionMenuUI = mapSelectionMenuUI;

            CurrentGameState = GameState.MainMenu;
        }

        public void StartGame(string mapPath, MapCamera mapCamera)
        {
            LoadMap(mapPath);

            TurnNumber = 1;

            foreach (GameUnit unit in Units)
            {
                unit.DisableExhaustedUnit();
            }

            ActiveUnit.ActivateUnit();
            ActiveUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            MapContext.SnapCursorToActiveUnit();
            MapContext.EndTurn();

            MapContext.UpdateWindowsEachTurn();
            ResultsUI.UpdateWindows();
            
            mapCamera.CenterCameraToCursor();
            
            CurrentGameState = GameState.InGame;
        }

        private void LoadMap(string mapPath)
        {
            TmxMapParser mapParser = new TmxMapParser(
                new TmxMap(mapPath),
                GameDriver.WorldTileSetTexture,
                GameDriver.TerrainTexture,
                GameDriver.UnitSprites,
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
            ITexture2D mapCursorTexture = GameDriver.MapCursorTexture;

            mapContext = new MapContext(
                new MapContainer(mapParser.LoadMapGrid(), mapCursorTexture),
                new MapUI(GameDriver.ScreenSize)
            );
        }

        private static void LoadInitiativeContext(TmxMapParser mapParser)
        {
            List<GameUnit> unitsFromMap = UnitClassBuilder.GenerateUnitsFromMap(
                mapParser.LoadUnits(),
                GameDriver.LargePortraitTextures,
                GameDriver.MediumPortraitTextures,
                GameDriver.SmallPortraitTextures
            );

            //Randomize the team that goes first
            InitiativeContext =
                new InitiativeContext(unitsFromMap, (GameDriver.Random.Next(2) == 0) ? Team.Blue : Team.Red);
        }


        public void SelectUnitAndStartMoving()
        {
            if (!TrySelectUnit()) return;
            StartMoving();
        }

        public void FinishMoving()
        {
            if (MapContext.OtherUnitExistsAtCursor()) return;
            MapContext.ProceedToNextState();

            MapContainer.ClearDynamicGrid();
            MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
            //TODO Open the action menu
        }

        public void StartAction()
        {
            //TODO Select option in the action menu
            MapContext.ProceedToNextState();
            MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            //If the selection is Basic Attack
            //Open the targeting grid
            MapContext.SelectedUnit = UnitSelector.SelectUnit(MapContainer.GetMapSliceAtCursor().UnitEntity);
            MapContext.GenerateTargetingGridAtUnit(new SpriteAtlas(
                new Texture2DWrapper(GameDriver.ActionTiles.MonoGameTexture),
                new Vector2(GameDriver.CellSize), 3));
        }

        public void StartCombat()
        {
            GameUnit attackingUnit = MapContext.SelectedUnit;
            GameUnit defendingUnit = UnitSelector.SelectUnit(MapContainer.GetMapSliceAtCursor().UnitEntity);

            if (MapContext.TargetUnitIsLegal(defendingUnit))
            {
                MapContainer.ClearDynamicGrid();
                BattleContext.StartNewCombat(attackingUnit,
                    MapContainer.GetMapSliceAtCoordinates(attackingUnit.UnitEntity.MapCoordinates),
                    defendingUnit,
                    MapContainer.GetMapSliceAtCoordinates(defendingUnit.UnitEntity.MapCoordinates));

                MapContext.SetPromptWindowText("Confirm End Turn");
                MapContext.ProceedToNextState();
            }
            //Skip the combat state if player selects the same unit
            else if (attackingUnit == defendingUnit)
            {
                MapContainer.ClearDynamicGrid();
                MapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                MapContext.ProceedToNextState();
                MapContext.SetPromptWindowText("Confirm End Turn");
                MapContext.ProceedToNextState();
            }
        }

        public void ContinueCombat()
        {
            switch (BattleContext.CurrentState)
            {
                case BattleContext.BattleState.Start:
                    if (BattleContext.TryProceedToNextState())
                    {
                        BattleContext.StartRollingDice();
                    }

                    break;
                case BattleContext.BattleState.RollDice:
                    if (BattleContext.TryProceedToNextState())
                    {
                        BattleContext.StartResolvingBlocks();
                    }

                    break;
                case BattleContext.BattleState.CountDice:
                    if (BattleContext.TryProceedToNextState())
                    {
                        BattleContext.StartResolvingDamage();
                    }

                    break;
                case BattleContext.BattleState.ResolveCombat:
                    if (BattleContext.TryProceedToNextState())
                    {
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
            switch (MapContext.CurrentTurnState)
            {
                case MapContext.TurnState.SelectUnit:
                    break;
                case MapContext.TurnState.UnitMoving:
                    break;
                case MapContext.TurnState.UnitDecidingAction:
                    break;
                case MapContext.TurnState.UnitTargeting:
                    oldZoom = mapCamera.CurrentZoom;
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
            ActiveUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Attack);
            MapContext.SlideCursorToActiveUnit();

            MapContext.EndTurn();
            TurnNumber++;

            if (!Units.TrueForAll(unit => unit.Stats.Hp <= 0))
            {
                EndTurnIfUnitIsDead();
            }

            MapContext.UpdateWindowsEachTurn();
            ResultsUI.UpdateWindows();
        }


        public void CancelMove()
        {
            MapContext.CancelMovement();
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
                Trace.WriteLine("Selecting unit: " + MapContext.SelectedUnit.Team + " " +
                                MapContext.SelectedUnit.Role);
                MapContext.ProceedToNextState();
                MapContext.GenerateMoveGrid(
                    MapContainer.MapCursor.MapCoordinates,
                    MapContext.SelectedUnit.Stats.MaxMv,
                    new SpriteAtlas(new Texture2DWrapper(GameDriver.ActionTiles.MonoGameTexture),
                        new Vector2(GameDriver.CellSize), 2));
            }
            else
            {
                Trace.WriteLine("No unit to select.");
            }
        }

        private bool TrySelectUnit()
        {
            //Select the unit. Store it somewhere.
            MapContext.SelectedUnit =
                UnitSelector.SelectUnit(
                    MapContainer.GetMapSliceAtCursor().UnitEntity);

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
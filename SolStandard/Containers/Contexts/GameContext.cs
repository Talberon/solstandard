using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;
using TiledSharp;

namespace SolStandard.Containers.Contexts
{
    public static class GameContext
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

        public static BattleContext BattleContext { get; private set; }
        public static Scenario Scenario { get; private set; }
        public static MapSelectContext MapSelectContext { get; private set; }
        public static GameMapContext GameMapContext { get; private set; }
        public static InitiativeContext InitiativeContext { get; private set; }
        public static StatusScreenView StatusScreenView { get; private set; }
        public static MainMenuView MainMenuView { get; private set; }
        private static float _oldZoom;

        public static GameState CurrentGameState;
        public static PlayerIndex ActivePlayer { get; set; }

        private const string MapDirectory = "Content/TmxMaps/";
        private const string MapSelectFile = "Map_Select_02.tmx";

        public static void Initialize(MainMenuView mainMenuView)
        {
            MainMenuView = mainMenuView;

            BattleContext = new BattleContext(new BattleView());

            LoadMapSelect();

            CurrentGameState = GameState.MainMenu;
            _oldZoom = MapCamera.CurrentZoom;

            ActivePlayer = PlayerIndex.One;
        }

        public static MapCursor MapCursor
        {
            get
            {
                switch (CurrentGameState)
                {
                    case GameState.MainMenu:
                        return MapSelectContext.MapContainer.MapCursor;
                    case GameState.ModeSelect:
                        return MapSelectContext.MapContainer.MapCursor;
                    case GameState.ArmyDraft:
                        return MapSelectContext.MapContainer.MapCursor;
                    case GameState.MapSelect:
                        return MapSelectContext.MapContainer.MapCursor;
                    case GameState.PauseScreen:
                        return GameMapContext.MapContainer.MapCursor;
                    case GameState.InGame:
                        return GameMapContext.MapContainer.MapCursor;
                    case GameState.Results:
                        return GameMapContext.MapContainer.MapCursor;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static MapCamera MapCamera
        {
            get
            {
                switch (CurrentGameState)
                {
                    case GameState.MainMenu:
                        return MapSelectContext.MapContainer.MapCamera;
                    case GameState.ModeSelect:
                        return MapSelectContext.MapContainer.MapCamera;
                    case GameState.ArmyDraft:
                        return MapSelectContext.MapContainer.MapCamera;
                    case GameState.MapSelect:
                        return MapSelectContext.MapContainer.MapCamera;
                    case GameState.PauseScreen:
                        return GameMapContext.MapContainer.MapCamera;
                    case GameState.InGame:
                        return GameMapContext.MapContainer.MapCamera;
                    case GameState.Results:
                        return GameMapContext.MapContainer.MapCamera;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static List<GameUnit> Units
        {
            get { return InitiativeContext.InitiativeList; }
        }

        public static GameUnit ActiveUnit
        {
            get { return InitiativeContext.CurrentActiveUnit; }
        }

        public static void StartGame(string mapPath, Scenario scenario)
        {
            Scenario = scenario;

            LoadMap(mapPath);

            foreach (GameUnit unit in Units)
            {
                unit.DisableExhaustedUnit();
            }

            ActiveUnit.ActivateUnit();
            GameMapContext.ResetCursorToActiveUnit();
            MapSelectContext.MapContainer.MapCamera.SnapCameraCenterToCursor();
            GameMapContext.EndTurn();

            GameMapContext.UpdateWindowsEachTurn();
            StatusScreenView.UpdateWindows();

            CurrentGameState = GameState.InGame;
        }

        public static void LoadMapSelect()
        {
            const string mapPath = MapDirectory + MapSelectFile;

            TmxMapParser mapParser = new TmxMapParser(
                new TmxMap(mapPath),
                AssetManager.OverworldTexture,
                AssetManager.EntitiesTexture,
                AssetManager.UnitSprites,
                GameDriver.TmxObjectTypeDefaults);

            MapSelectContext = new MapSelectContext(new MapSelectScreenView(),
                new MapContainer(mapParser.LoadMapGrid(), AssetManager.MapCursorTexture));

            MapCursor.SnapCursorToCoordinates(MapSelectContext.MapCenter);
            MapCamera.CenterCameraToCursor();

            LoadInitiativeContext(mapParser);

            CurrentGameState = GameState.MapSelect;
        }

        private static void LoadMap(string mapFile)
        {
            string mapPath = MapDirectory + mapFile;

            TmxMapParser mapParser = new TmxMapParser(
                new TmxMap(mapPath),
                AssetManager.OverworldTexture,
                AssetManager.EntitiesTexture,
                AssetManager.UnitSprites,
                GameDriver.TmxObjectTypeDefaults
            );

            LoadMapContext(mapParser);
            LoadInitiativeContext(mapParser);
            LoadStatusUI();
        }

        private static void LoadStatusUI()
        {
            StatusScreenView = new StatusScreenView();
        }

        private static void LoadMapContext(TmxMapParser mapParser)
        {
            ITexture2D mapCursorTexture = AssetManager.MapCursorTexture;

            GameMapContext = new GameMapContext(
                new MapContainer(mapParser.LoadMapGrid(), mapCursorTexture),
                new GameMapView()
            );
        }

        private static void LoadInitiativeContext(TmxMapParser mapParser)
        {
            List<GameUnit> unitsFromMap = UnitGenerator.GenerateUnitsFromMap(
                mapParser.LoadUnits(),
                mapParser.LoadMapLoot(),
                AssetManager.LargePortraitTextures,
                AssetManager.MediumPortraitTextures,
                AssetManager.SmallPortraitTextures
            );

            //Randomize the team that goes first
            InitiativeContext =
                new InitiativeContext(unitsFromMap, (GameDriver.Random.Next(1) == 0) ? Team.Blue : Team.Red);
        }

        public static void UpdateCamera()
        {
            MapCamera.UpdateEveryFrame();
        }
    }
}
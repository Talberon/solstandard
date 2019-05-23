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
            NetworkMenu,
            ArmyDraft,
            Deployment,
            MapSelect,
            PauseScreen,
            InGame,
            Results,
            Codex,
            ItemPreview,
            Credits
        }

        public static readonly Color PositiveColor = new Color(30, 200, 30);
        public static readonly Color NegativeColor = new Color(250, 10, 10);
        public static readonly Color NeutralColor = new Color(255, 250, 250);

        private const string MapDirectory = "Content/TmxMaps/";
        private const string MapSelectFile = "Map_Select_03.tmx";

        public static BattleContext BattleContext { get; private set; }
        public static Scenario Scenario { get; private set; }
        public static MapSelectContext MapSelectContext { get; private set; }
        public static GameMapContext GameMapContext { get; private set; }
        public static InitiativeContext InitiativeContext { get; private set; }
        public static StatusScreenView StatusScreenView { get; private set; }
        public static MainMenuView MainMenuView { get; private set; }
        public static NetworkMenuView NetworkMenuView { get; private set; }
        public static DraftContext DraftContext { get; private set; }
        public static DeploymentContext DeploymentContext { get; private set; }
        public static CodexContext CodexContext { get; private set; }
        public static CreditsContext CreditsContext { get; private set; }

        public static GameState CurrentGameState;

        public static PlayerIndex ActivePlayer
        {
            get
            {
                switch (CurrentGameState)
                {
                    case GameState.MainMenu:
                        return PlayerIndex.One;
                    case GameState.NetworkMenu:
                        return PlayerIndex.One;
                    case GameState.ArmyDraft:
                        return GetPlayerForTeam(DraftContext.CurrentTurn);
                    case GameState.Deployment:
                        return GetPlayerForTeam(DeploymentContext.CurrentTurn);
                    case GameState.MapSelect:
                        return GetPlayerForTeam(InitiativeContext.CurrentActiveTeam);
                    case GameState.PauseScreen:
                        return GetPlayerForTeam(InitiativeContext.CurrentActiveTeam);
                    case GameState.InGame:
                        return GetPlayerForTeam(InitiativeContext.CurrentActiveTeam);
                    case GameState.Codex:
                        return GetPlayerForTeam(InitiativeContext.CurrentActiveTeam);
                    case GameState.Results:
                        return PlayerIndex.Four;
                    case GameState.Credits:
                        return PlayerIndex.One;
                    case GameState.ItemPreview:
                        return GetPlayerForTeam(InitiativeContext.CurrentActiveTeam);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void Initialize(MainMenuView mainMenuView, NetworkMenuView networkMenuView)
        {
            MusicBox.PlayLoop(AssetManager.MusicTracks.Find(track => track.Name.Contains("MapSelect")), 0.3f);
            MainMenuView = mainMenuView;
            NetworkMenuView = networkMenuView;
            BattleContext = new BattleContext(new BattleView());
            DraftContext = new DraftContext();
            CodexContext = new CodexContext();
            CreditsContext = new CreditsContext(new CreditsView());
            LoadMapSelect();
            CurrentGameState = GameState.MainMenu;
        }

        public static MapCursor MapCursor
        {
            get
            {
                switch (CurrentGameState)
                {
                    case GameState.MainMenu:
                        return MapSelectContext.MapContainer.MapCursor;
                    case GameState.NetworkMenu:
                        return MapSelectContext.MapContainer.MapCursor;
                    case GameState.MapSelect:
                        return MapSelectContext.MapContainer.MapCursor;
                    case GameState.Deployment:
                        return GameMapContext.MapContainer.MapCursor;
                    case GameState.ArmyDraft:
                        return GameMapContext.MapContainer.MapCursor;
                    case GameState.PauseScreen:
                        return GameMapContext.MapContainer.MapCursor;
                    case GameState.InGame:
                        return GameMapContext.MapContainer.MapCursor;
                    case GameState.Results:
                        return GameMapContext.MapContainer.MapCursor;
                    case GameState.ItemPreview:
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
                    case GameState.NetworkMenu:
                        return MapSelectContext.MapContainer.MapCamera;
                    case GameState.MapSelect:
                        return MapSelectContext.MapContainer.MapCamera;
                    case GameState.Deployment:
                        return GameMapContext.MapContainer.MapCamera;
                    case GameState.ArmyDraft:
                        return GameMapContext.MapContainer.MapCamera;
                    case GameState.PauseScreen:
                        return GameMapContext.MapContainer.MapCamera;
                    case GameState.InGame:
                        return GameMapContext.MapContainer.MapCamera;
                    case GameState.Results:
                        return GameMapContext.MapContainer.MapCamera;
                    case GameState.ItemPreview:
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

        public static void CenterCursorAndCamera()
        {
            MapCursor.SnapCursorToCoordinates(new Vector2(
                (int) (MapContainer.MapGridSize.X / 2),
                (int) (MapContainer.MapGridSize.Y / 2))
            );
            MapCamera.CenterCameraToCursor();
        }

        public static void LoadMapAndScenario(string mapPath, Scenario scenario)
        {
            Scenario = scenario;
            LoadMap(mapPath);
        }

        public static void StartNewDeployment(List<GameUnit> blueArmy, List<GameUnit> redArmy, Team firstTurn)
        {
            DeploymentContext = new DeploymentContext(blueArmy, redArmy, GameMapContext.MapContainer, firstTurn);
            CurrentGameState = GameState.Deployment;
        }

        public static void StartGame(string mapPath, Scenario scenario)
        {
            Scenario = scenario;

            LoadMap(mapPath);

            CurrentGameState = GameState.InGame;

            foreach (GameUnit unit in Units)
            {
                unit.DisableExhaustedUnit();
            }

            InitiativeContext.StartFirstTurn();
            GameMapContext.ResetTurnState();

            GameMapContext.UpdateWindowsEachTurn();
            StatusScreenView.UpdateWindows();
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

            //Player 1 (Blue) always controls map select screen
            LoadInitiativeContext(mapParser, Team.Blue);

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
            LoadInitiativeContext(mapParser, (GameDriver.Random.Next(2) == 0) ? Team.Blue : Team.Red);
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
                new MapContainer(mapParser.LoadMapGrid(), mapCursorTexture, mapParser.LoadSummons()),
                new GameMapView()
            );
        }

        private static void LoadInitiativeContext(TmxMapParser mapParser, Team firstTeam)
        {
            List<GameUnit> unitsFromMap =
                UnitGenerator.GenerateUnitsFromMap(mapParser.LoadUnits(), mapParser.LoadMapLoot());

            InitiativeContext = new InitiativeContext(unitsFromMap, firstTeam);
        }

        public static void UpdateCamera()
        {
            MapCamera.UpdateEveryFrame();
        }

        public static void GoToMainMenuIfGameIsOver()
        {
            if (!Scenario.GameIsOver) return;

            if (GameDriver.ConnectedAsClient || GameDriver.ConnectedAsServer)
            {
                GameDriver.ConnectionManager.CloseServer();
                GameDriver.ConnectionManager.DisconnectClient();
            }

            AssetManager.MenuConfirmSFX.Play();
            Initialize(MainMenuView, NetworkMenuView);
        }

        public static PlayerIndex GetPlayerForTeam(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return PlayerIndex.One;
                case Team.Red:
                    return PlayerIndex.Two;
                case Team.Creep:
                    return PlayerIndex.Three;
                default:
                    throw new ArgumentOutOfRangeException("team", team, null);
            }
        }
    }
}
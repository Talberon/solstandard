using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using NLog;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Containers.View;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;
using SolStandard.Utility.Exceptions;
using SolStandard.Utility.Monogame;
using TiledSharp;

namespace SolStandard.Containers.Contexts
{
    public static class GameContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public enum GameState
        {
            EULAConfirm,
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
            Credits,
            ControlConfig,
            HowToPlay
        }

        public static readonly Color PositiveColor = new Color(30, 200, 30);
        public static readonly Color NegativeColor = new Color(250, 10, 10);
        public static readonly Color NeutralColor = new Color(255, 255, 255);

        private const string MapDirectory = @"Content/TmxMaps/";
        private const string MapSelectFile = @"Map_Select_06.tmx";

        public static BattleContext BattleContext { get; private set; }
        public static Scenario Scenario { get; private set; }
        public static MapSelectContext MapSelectContext { get; private set; }
        public static GameMapContext GameMapContext { get; private set; }
        public static InitiativeContext InitiativeContext { get; private set; }
        public static StatusScreenView StatusScreenView { get; private set; }
        public static MainMenuView MainMenuView { get; private set; }
        public static NetworkMenuView NetworkMenuView { get; private set; }
        public static BackgroundView BackgroundView { get; private set; }
        public static DraftContext DraftContext { get; private set; }
        public static DeploymentContext DeploymentContext { get; private set; }
        public static CodexContext CodexContext { get; private set; }
        public static CreditsContext CreditsContext { get; private set; }
        public static ControlConfigContext ControlConfigContext { get; private set; }
        public static EULAContext EULAContext { get; private set; }
        public static HowToPlayContext HowToPlayContext { get; private set; }

        public static Team P1Team { get; private set; }
        public static Team P2Team => (P1Team == Team.Blue) ? Team.Red : Team.Blue;

        public static GameState CurrentGameState;

        public static PlayerIndex ActivePlayer => CurrentGameState switch
        {
            GameState.EULAConfirm => PlayerIndex.One,
            GameState.MainMenu => PlayerIndex.One,
            GameState.NetworkMenu => PlayerIndex.One,
            GameState.MapSelect => PlayerIndex.One,
            GameState.ArmyDraft => GetPlayerForTeam(DraftContext.CurrentTurn),
            GameState.Deployment => GetPlayerForTeam(DeploymentContext.CurrentTurn),
            GameState.PauseScreen => GetPlayerForTeam(ActiveTeam),
            GameState.InGame => GetPlayerForTeam(ActiveTeam),
            GameState.Codex => GetPlayerForTeam(CodexContext.CurrentTeam),
            GameState.Results => GetPlayerForTeam(ActiveTeam),
            GameState.Credits => PlayerIndex.One,
            GameState.ItemPreview => GetPlayerForTeam(ActiveTeam),
            GameState.ControlConfig => ((InitiativeContext != null)
                ? GetPlayerForTeam(ActiveTeam)
                : PlayerIndex.One),
            GameState.HowToPlay => GetPlayerForTeam(ActiveTeam),
            _ => throw new ArgumentOutOfRangeException()
        };

        public static void Initialize(MainMenuView mainMenuView, NetworkMenuView networkMenuView)
        {
            MusicBox.PlayLoop(AssetManager.MusicTracks.Find(track => track.Name.EndsWith("MapSelectTheme")));
            MainMenuView = mainMenuView;
            NetworkMenuView = networkMenuView;
            EULAContext = new EULAContext();
            BattleContext = new BattleContext(new BattleView());
            DraftContext = new DraftContext();
            CodexContext = new CodexContext();
            CreditsContext = new CreditsContext(new CreditsView());
            ControlConfigContext = new ControlConfigContext(new ControlConfigView());
            BackgroundView = new BackgroundView();
            HowToPlayContext = new HowToPlayContext();
            LoadMapSelect();
            CurrentGameState = GameState.EULAConfirm;
            P1Team = Team.Red;

            if (EULAContext.EULAConfirmed) CurrentGameState = GameState.MainMenu;
        }

        public static void SetP1Team(Team team)
        {
            if (team != Team.Red && team != Team.Blue) throw new InvalidTeamException();
            P1Team = team;
            GameDriver.InitializeControlMappers(team);
            MapSelectContext.MapSelectScreenView.UpdateTeamSelectWindow();
            AssetManager.MapUnitCancelSFX.Play();
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(5));
        }

        public static MapCursor MapCursor => CurrentGameState switch
        {
            GameState.EULAConfirm => MapSelectContext.MapContainer.MapCursor,
            GameState.MainMenu => MapSelectContext.MapContainer.MapCursor,
            GameState.NetworkMenu => MapSelectContext.MapContainer.MapCursor,
            GameState.MapSelect => MapSelectContext.MapContainer.MapCursor,
            GameState.Deployment => GameMapContext.MapContainer.MapCursor,
            GameState.ArmyDraft => GameMapContext.MapContainer.MapCursor,
            GameState.PauseScreen => GameMapContext.MapContainer.MapCursor,
            GameState.InGame => GameMapContext.MapContainer.MapCursor,
            GameState.Results => GameMapContext.MapContainer.MapCursor,
            GameState.ItemPreview => GameMapContext.MapContainer.MapCursor,
            GameState.Codex => null,
            GameState.Credits => null,
            GameState.HowToPlay => MapSelectContext.MapContainer.MapCursor,
            _ => throw new ArgumentOutOfRangeException()
        };

        public static MapCamera MapCamera => CurrentGameState switch
        {
            GameState.EULAConfirm => MapSelectContext.MapContainer.MapCamera,
            GameState.MainMenu => MapSelectContext.MapContainer.MapCamera,
            GameState.NetworkMenu => MapSelectContext.MapContainer.MapCamera,
            GameState.MapSelect => MapSelectContext.MapContainer.MapCamera,
            GameState.Deployment => GameMapContext.MapContainer.MapCamera,
            GameState.ArmyDraft => GameMapContext.MapContainer.MapCamera,
            GameState.PauseScreen => GameMapContext.MapContainer.MapCamera,
            GameState.InGame => GameMapContext.MapContainer.MapCamera,
            GameState.Results => GameMapContext.MapContainer.MapCamera,
            GameState.ItemPreview => GameMapContext.MapContainer.MapCamera,
            GameState.Codex => GameMapContext.MapContainer.MapCamera,
            GameState.Credits => MapSelectContext.MapContainer.MapCamera,
            GameState.HowToPlay => MapSelectContext.MapContainer.MapCamera,
            _ => throw new ArgumentOutOfRangeException()
        };

        public static List<GameUnit> Units => InitiativeContext.Units;

        public static GameUnit ActiveUnit => InitiativeContext.CurrentActiveUnit;

        public static Team ActiveTeam => CurrentGameState switch
        {
            GameState.ArmyDraft => DraftContext.CurrentTurn,
            GameState.Deployment => DeploymentContext.CurrentTurn,
            _ => InitiativeContext.CurrentActiveTeam
        };

        public static void CenterCursorAndCamera()
        {
            MapCursor.SnapCameraAndCursorToCoordinates(new Vector2(
                (int) (MapContainer.MapGridSize.X / 2),
                (int) (MapContainer.MapGridSize.Y / 2))
            );
            MapCamera.CenterCameraToCursor();
        }

        public static void LoadMapAndScenario(string mapPath, Scenario scenario, Team firstTeam)
        {
            Scenario = scenario;
            LoadMap(mapPath, firstTeam);
        }

        public static void StartNewDeployment(List<GameUnit> blueArmy, List<GameUnit> redArmy, Team firstTurn)
        {
            DeploymentContext = new DeploymentContext(blueArmy, redArmy, GameMapContext.MapContainer, firstTurn);
            CurrentGameState = GameState.Deployment;
        }

        public static void StartGame(string mapPath, Scenario scenario, Team firstTeam)
        {
            Scenario = scenario;

            LoadMap(mapPath, firstTeam);

            CurrentGameState = GameState.InGame;

            foreach (GameUnit unit in Units)
            {
                unit.ExhaustAndDisableUnit();
            }

            GameMapContext.UpdateWindowsEachTurn();
            InitiativeContext.StartFirstTurn();
            GameMapContext.ResetTurnState();

            StatusScreenView.UpdateWindows();
        }

        public static void LoadMapSelect()
        {
            GlobalEventQueue.ClearEventQueue();
            MapContainer.ClearToasts();
            Bank.ResetBank();

            string mapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MapDirectory, MapSelectFile);

            TmxMapParser mapParser = new TmxMapParser(
                new TmxMap(mapPath),
                AssetManager.OverworldTexture,
                AssetManager.EntitiesTexture,
                AssetManager.UnitSprites,
                GameDriver.TmxObjectTypeDefaults);

            MapSelectContext = new MapSelectContext(new MapSelectScreenView(),
                new MapContainer(mapParser.LoadMapGrid(), AssetManager.MapCursorTexture));

            MapCursor.SnapCameraAndCursorToCoordinates(MapSelectContext.MapCenter);
            MapCamera.SnapCameraCenterToCursor();
            MapCamera.SetZoomLevel(MapCamera.ZoomLevel.Far);

            //Player 1 (Blue) always controls map select screen
            LoadInitiativeContext(mapParser, Team.Red);

            CurrentGameState = GameState.MapSelect;
        }

        private static void LoadMap(string mapFile, Team firstTeam)
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

            LoadInitiativeContext(mapParser, firstTeam);

            InjectCreepsIntoSpawnTiles();
            LoadStatusUI();
        }

        private static void InjectCreepsIntoSpawnTiles()
        {
            List<CreepEntity> summons = GameMapContext.MapContainer.MapSummons;

            List<CreepDeployTile> creepDeployTiles = MapContainer.GetMapEntities()
                .Where(entity => entity.GetType() == typeof(CreepDeployTile)).Cast<CreepDeployTile>().ToList();

            foreach (CreepDeployTile creepDeployTile in creepDeployTiles)
            {
                List<CreepEntity> eligibleCreeps =
                    summons.Where(creep => creep.CreepPool == creepDeployTile.CreepPool).ToList();

                CreepEntity randomSummon = eligibleCreeps[GameDriver.Random.Next(eligibleCreeps.Count)];

                InjectCreepIntoTile(randomSummon, creepDeployTile);

                if (!creepDeployTile.CopyCreep) summons.Remove(randomSummon);
            }
        }

        private static void InjectCreepIntoTile(CreepEntity randomSummon, MapElement creepDeployTile)
        {
            Logger.Debug($"Injecting {randomSummon.Name} at {creepDeployTile.MapCoordinates}");

            GameUnit creepToSpawn =
                UnitGenerator.BuildUnitFromProperties(
                    randomSummon.Name,
                    randomSummon.Team,
                    randomSummon.Role,
                    randomSummon.IsCommander,
                    randomSummon.Copy()
                );

            creepToSpawn.UnitEntity.SnapToCoordinates(creepDeployTile.MapCoordinates);
            creepToSpawn.ExhaustAndDisableUnit();
            Units.Add(creepToSpawn);

            MapContainer.GameGrid[(int) Layer.Entities]
                [(int) creepDeployTile.MapCoordinates.X, (int) creepDeployTile.MapCoordinates.Y] = null;
        }

        private static void LoadStatusUI()
        {
            StatusScreenView = new StatusScreenView();
        }

        private static void LoadMapContext(TmxMapParser mapParser)
        {
            GameMapContext = new GameMapContext(
                new MapContainer(
                    mapParser.LoadMapGrid(),
                    AssetManager.MapCursorTexture,
                    mapParser.LoadSummons(),
                    mapParser.LoadMapLoot()
                ),
                new GameMapView()
            );
        }

        private static void LoadInitiativeContext(TmxMapParser mapParser, Team firstTeam)
        {
            List<GameUnit> unitsFromMap = UnitGenerator.GenerateUnitsFromMap(mapParser.LoadUnits());

            InitiativeContext = new InitiativeContext(unitsFromMap, firstTeam);
        }

        public static void UpdateCamera()
        {
            MapCamera.UpdateEveryFrame();
        }

        public static void GoToMainMenuIfGameIsOver()
        {
            if (!Scenario.GameIsOver) return;

            GlobalEventQueue.QueueSingleEvent(new ResetGameEvent());
        }

        private static PlayerIndex GetPlayerForTeam(Team team)
        {
            if (P1Team == team) return PlayerIndex.One;
            if (P2Team == team) return PlayerIndex.Two;
            return PlayerIndex.Three;
        }
    }
}
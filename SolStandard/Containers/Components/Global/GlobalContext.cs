using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using NLog;
using SolStandard.Containers.Components.Codex;
using SolStandard.Containers.Components.Credits;
using SolStandard.Containers.Components.Deployment;
using SolStandard.Containers.Components.Draft;
using SolStandard.Containers.Components.EULA;
using SolStandard.Containers.Components.HowToPlay;
using SolStandard.Containers.Components.InputRemapping;
using SolStandard.Containers.Components.LevelSelect;
using SolStandard.Containers.Components.MainMenu;
using SolStandard.Containers.Components.Network;
using SolStandard.Containers.Components.SplashScreen;
using SolStandard.Containers.Components.World;
using SolStandard.Containers.Components.World.SubContext.Battle;
using SolStandard.Containers.Components.World.SubContext.Initiative;
using SolStandard.Containers.Components.World.SubContext.Status;
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
using TiledSharp;

namespace SolStandard.Containers.Components.Global
{
    public static class GlobalContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public enum GameState
        {
            SplashScreen,
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

        private static readonly string MapDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, @"Content/TmxMaps/");

        private const string MapSelectFile = @"Map_Select_06.tmx";

        public static CombatPhase CombatPhase { get; private set; }
        public static Scenario.Scenario Scenario { get; private set; }
        public static MapSelectContext MapSelectContext { get; private set; }
        public static WorldContext WorldContext { get; private set; }
        public static InitiativePhase InitiativePhase { get; private set; }
        public static StatusScreenHUD StatusScreenHUD { get; private set; }
        public static MainMenuHUD MainMenuHUD { get; private set; }
        public static NetworkHUD NetworkHUD { get; private set; }
        public static StaticBackgroundView StaticBackgroundView { get; private set; }
        public static DraftContext DraftContext { get; private set; }
        public static DeploymentContext DeploymentContext { get; private set; }
        public static CodexContext CodexContext { get; private set; }
        public static CreditsContext CreditsContext { get; private set; }
        public static ControlConfigContext ControlConfigContext { get; private set; }
        public static SplashScreenContext SplashScreenContext { get; private set; }
        public static EULAContext EULAContext { get; private set; }
        public static HowToPlayContext HowToPlayContext { get; private set; }

        public static Team P1Team { get; private set; }
        public static Team P2Team => (P1Team == Team.Blue) ? Team.Red : Team.Blue;

        public static GameState CurrentGameState;

        public static PlayerIndex ActivePlayer => CurrentGameState switch
        {
            GameState.SplashScreen => PlayerIndex.One,
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
            GameState.ControlConfig => ((InitiativePhase != null)
                ? GetPlayerForTeam(ActiveTeam)
                : PlayerIndex.One),
            GameState.HowToPlay => GetPlayerForTeam(ActiveTeam),
            _ => throw new ArgumentOutOfRangeException()
        };

        public static void Initialize(MainMenuHUD mainMenuHUD, NetworkHUD networkHUD)
        {
            MainMenuHUD = mainMenuHUD;
            NetworkHUD = networkHUD;
            SplashScreenContext = new SplashScreenContext(new SplashScreenHUD());
            EULAContext = new EULAContext();
            CombatPhase = new CombatPhase(new CombatHUD());
            DraftContext = new DraftContext();
            CodexContext = new CodexContext();
            CreditsContext = new CreditsContext(new CreditsHUD());
            ControlConfigContext = new ControlConfigContext(new ControlConfigView());
            StaticBackgroundView = new StaticBackgroundView();
            HowToPlayContext = new HowToPlayContext();
            LoadMapSelect();
            CurrentGameState = GameState.SplashScreen;
            P1Team = Team.Red;
        }

        public static void SetP1Team(Team team)
        {
            if (team != Team.Red && team != Team.Blue) throw new InvalidTeamException();
            P1Team = team;
            GameDriver.InitializeControlMappers(team);
            MapSelectContext.MapSelectHUD.UpdateTeamSelectWindow();
            AssetManager.MapUnitCancelSFX.Play();
            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(5));
        }

        public static MapCursor MapCursor => CurrentGameState switch
        {
            GameState.SplashScreen => MapSelectContext.MapContainer.MapCursor,
            GameState.EULAConfirm => MapSelectContext.MapContainer.MapCursor,
            GameState.MainMenu => MapSelectContext.MapContainer.MapCursor,
            GameState.NetworkMenu => MapSelectContext.MapContainer.MapCursor,
            GameState.MapSelect => MapSelectContext.MapContainer.MapCursor,
            GameState.Deployment => WorldContext.MapContainer.MapCursor,
            GameState.ArmyDraft => WorldContext.MapContainer.MapCursor,
            GameState.PauseScreen => WorldContext.MapContainer.MapCursor,
            GameState.InGame => WorldContext.MapContainer.MapCursor,
            GameState.Results => WorldContext.MapContainer.MapCursor,
            GameState.ItemPreview => WorldContext.MapContainer.MapCursor,
            GameState.Codex => null,
            GameState.Credits => null,
            GameState.HowToPlay => MapSelectContext.MapContainer.MapCursor,
            _ => throw new ArgumentOutOfRangeException()
        };

        public static IMapCamera MapCamera => CurrentGameState switch
        {
            GameState.SplashScreen => MapSelectContext.MapContainer.MapCamera,
            GameState.EULAConfirm => MapSelectContext.MapContainer.MapCamera,
            GameState.MainMenu => MapSelectContext.MapContainer.MapCamera,
            GameState.NetworkMenu => MapSelectContext.MapContainer.MapCamera,
            GameState.MapSelect => MapSelectContext.MapContainer.MapCamera,
            GameState.Deployment => WorldContext.MapContainer.MapCamera,
            GameState.ArmyDraft => WorldContext.MapContainer.MapCamera,
            GameState.PauseScreen => WorldContext.MapContainer.MapCamera,
            GameState.InGame => WorldContext.MapContainer.MapCamera,
            GameState.Results => WorldContext.MapContainer.MapCamera,
            GameState.ItemPreview => WorldContext.MapContainer.MapCamera,
            GameState.Codex => WorldContext.MapContainer.MapCamera,
            GameState.Credits => MapSelectContext.MapContainer.MapCamera,
            GameState.HowToPlay => MapSelectContext.MapContainer.MapCamera,
            _ => throw new ArgumentOutOfRangeException()
        };

        public static List<GameUnit> Units => InitiativePhase.Units;

        public static GameUnit ActiveUnit => InitiativePhase.CurrentActiveUnit;

        public static Team ActiveTeam => CurrentGameState switch
        {
            GameState.ArmyDraft => DraftContext.CurrentTurn,
            GameState.Deployment => DeploymentContext.CurrentTurn,
            _ => InitiativePhase.CurrentActiveTeam
        };

        public static void CenterCursorAndCamera()
        {
            MapCursor.SnapCameraAndCursorToCoordinates(new Vector2(
                (int) (MapContainer.MapGridSize.X / 2),
                (int) (MapContainer.MapGridSize.Y / 2))
            );
            MapCamera.CenterCameraToCursor();
        }

        public static void LoadMapAndScenario(string mapPath, Scenario.Scenario scenario, Team firstTeam)
        {
            Scenario = scenario;
            LoadMap(mapPath, firstTeam);
        }

        public static void StartNewDeployment(List<GameUnit> blueArmy, List<GameUnit> redArmy, Team firstTurn)
        {
            DeploymentContext = new DeploymentContext(blueArmy, redArmy, WorldContext.MapContainer, firstTurn);
            CurrentGameState = GameState.Deployment;
        }

        public static void StartGame(string mapPath, Scenario.Scenario scenario, Team firstTeam)
        {
            Scenario = scenario;

            LoadMap(mapPath, firstTeam);

            CurrentGameState = GameState.InGame;

            bool isSinglePlayer = Units.TrueForAll(unit => unit.Team == Team.Red || unit.Team == Team.Creep) ||
                                  Units.TrueForAll(unit => unit.Team == Team.Blue || unit.Team == Team.Creep);

            if (!isSinglePlayer && !CreepPreferences.Instance.CreepsCanSpawn)
            {
                foreach (GameUnit creep in Units.Where(unit => unit.Team == Team.Creep))
                {
                    creep.KillUnit();
                }
            }
            
            foreach (GameUnit unit in Units)
            {
                unit.ExhaustAndDisableUnit();
            }

            WorldContext.UpdateWindowsEachTurn();
            InitiativePhase.StartFirstTurn();
            WorldContext.ResetTurnState();

            StatusScreenHUD.UpdateWindows();
        }

        public static void LoadMapSelect()
        {
            GlobalEventQueue.ClearEventQueue();
            MapContainer.ClearToasts();
            Bank.ResetBank();

            string mapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, MapDirectory, MapSelectFile);

            var mapParser = new TmxMapParser(
                new TmxMap(mapPath),
                AssetManager.OverworldTexture,
                AssetManager.EntitiesTexture,
                AssetManager.UnitSprites,
                GameDriver.TmxObjectTypeDefaults);

            MapSelectContext = new MapSelectContext(new MapSelectHUD(),
                new MapContainer(mapParser.LoadMapGrid(), AssetManager.MapCursorTexture));
            CenterCursorAndCamera();
            MapCamera.SetZoomLevel(IMapCamera.ZoomLevel.Far);

            //Player 1 (Red) always controls map select screen
            LoadInitiativeContext(mapParser, Team.Red);

            CurrentGameState = GameState.MapSelect;
        }

        private static void LoadMap(string mapFile, Team firstTeam)
        {
            string mapPath = MapDirectory + @mapFile;

            var mapParser = new TmxMapParser(
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
            List<CreepEntity> summons = WorldContext.MapContainer.MapSummons;

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
            StatusScreenHUD = new StatusScreenHUD();
        }

        private static void LoadMapContext(TmxMapParser mapParser)
        {
            WorldContext = new WorldContext(
                new MapContainer(
                    mapParser.LoadMapGrid(),
                    AssetManager.MapCursorTexture,
                    mapParser.LoadSummons(),
                    mapParser.LoadMapLoot()
                ),
                new WorldHUD()
            );
        }

        private static void LoadInitiativeContext(TmxMapParser mapParser, Team firstTeam)
        {
            List<GameUnit> unitsFromMap = UnitGenerator.GenerateUnitsFromMap(mapParser.LoadUnits());

            InitiativePhase = new InitiativePhase(unitsFromMap, firstTeam);
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
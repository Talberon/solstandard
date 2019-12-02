using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Inputs.Gamepad;
using SolStandard.Utility.Inputs.KeyboardInput;
using SolStandard.Utility.Monogame;
using SolStandard.Utility.Network;
using SolStandard.Utility.System;

namespace SolStandard
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameDriver : Game
    {
        // ReSharper disable once NotAccessedField.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private GraphicsDeviceManager graphics;

        public static readonly IFileIO SystemFileIO = new WindowsFileIO();

        //Project Site
        public const string SolStandardUrl = "https://solstandard.talberon.com";

        //Tile Size of Sprites
        public const int CellSize = 32;
        public static readonly Vector2 CellSizeVector = new Vector2(CellSize);
        public const string TmxObjectTypeDefaults = "Content/TmxMaps/objecttypes.xml";

        private static readonly Color BackgroundColor = new Color(20, 11, 40);
        private static readonly Color ActionFade = new Color(0, 0, 0, 190);
        public static Random Random = new Random();
        public static Vector2 ScreenSize { get; private set; }
        public static ConnectionManager ConnectionManager;

        private SpriteBatch spriteBatch;
        private static ControlMapper _blueTeamControlMapper;
        private static ControlMapper _redTeamControlMapper;

        private static bool _quitting;
        public static GameControlParser KeyboardParser;
        public static GameControlParser P1GamepadParser;
        public static GameControlParser P2GamepadParser;

        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            UseDefaultResolution();
//            UseBorderlessFullscreen();
            Content.RootDirectory = "Content";
        }

        public void UseDefaultResolution()
        {
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();
            Window.Position = new Point(0, 50);
            Window.IsBorderless = false;
            Window.AllowUserResizing = true;
        }

        public void UseBorderlessFullscreen()
        {
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            Window.IsBorderless = true;
            Window.Position = new Point(0, 0);
            Window.AllowUserResizing = false;
        }


        /// <summary>
        /// Starts a new game by generating a new map
        /// </summary>
        public static void NewGame(string mapName, Scenario scenario, Team firstTeam)
        {
            GameContext.StartGame(mapName, scenario, firstTeam);
        }

        public static void HostGame()
        {
            //Start Server
            string serverIP = ConnectionManager.StartServer();
            GameContext.NetworkMenuView.UpdateStatus(serverIP, true, serverIP != null);
            GameContext.NetworkMenuView.GenerateHostMenu();
            GameContext.NetworkMenuView.RemoveDialMenu();
            GameContext.CurrentGameState = GameContext.GameState.NetworkMenu;
        }

        public static void JoinGame(string serverIPAddress = "127.0.0.1")
        {
            //Start Client
            ConnectionManager.StartClient(serverIPAddress, ConnectionManager.NetworkPort);
            GameContext.NetworkMenuView.UpdateStatus(serverIPAddress, false);
            GameContext.NetworkMenuView.GenerateDialMenu();
            GameContext.NetworkMenuView.RemoveHostMenu();
            GameContext.CurrentGameState = GameContext.GameState.NetworkMenu;
        }

        public static bool ConnectedAsServer => ConnectionManager.ConnectedAsServer;

        public static bool ConnectedAsClient => ConnectionManager.ConnectedAsClient;

        public static void InitializeControlMappers(Team playerOneTeam)
        {
            switch (playerOneTeam)
            {
                case Team.Blue:
                    _blueTeamControlMapper = new MultiControlParser(KeyboardParser, P1GamepadParser);
                    _redTeamControlMapper = new MultiControlParser(KeyboardParser, P2GamepadParser);
                    break;
                case Team.Red:
                    _redTeamControlMapper = new MultiControlParser(KeyboardParser, P1GamepadParser);
                    _blueTeamControlMapper = new MultiControlParser(KeyboardParser, P2GamepadParser);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerOneTeam), playerOneTeam, null);
            }
        }

        public static void QuitGame()
        {
            _quitting = true;
        }


        private static void CleanTmxFiles()
        {
            const string tmxPath = "Content/TmxMaps/";
            Regex tmxName = new Regex("([\\w])+.tmx");

            foreach (string tmxFile in Directory.GetFiles(tmxPath).Where(filename => tmxName.IsMatch(filename)))
            {
                string text = File.ReadAllText(tmxFile);
                text = text.Replace("tile/", "tile gid=\"0\"/");
                File.WriteAllText(tmxFile, text);
            }
        }

        private static ControlMapper GetControlMapperForPlayer(PlayerIndex playerIndex)
        {
            return playerIndex switch
            {
                PlayerIndex.One => ((GameContext.P1Team == Team.Blue) ? _blueTeamControlMapper : _redTeamControlMapper),
                PlayerIndex.Two => ((GameContext.P2Team == Team.Blue) ? _blueTeamControlMapper : _redTeamControlMapper),
                _ => GetControlMapperForPlayer(PlayerIndex.One)
            };
        }


        private static void InitializeControllers()
        {
            IController loadedKeyboardConfig =
                SystemFileIO.Load<IController>(ControlConfigContext.KeyboardConfigFileName);
            KeyboardParser = new GameControlParser(loadedKeyboardConfig ?? new KeyboardController());

            IController loadedP1GamepadConfig =
                SystemFileIO.Load<IController>(ControlConfigContext.P1GamepadConfigFileName);
            P1GamepadParser = new GameControlParser(loadedP1GamepadConfig ?? new GamepadController(PlayerIndex.One));

            IController loadedP2GamepadConfig =
                SystemFileIO.Load<IController>(ControlConfigContext.P2GamepadConfigFileName);
            P2GamepadParser = new GameControlParser(loadedP2GamepadConfig ?? new GamepadController(PlayerIndex.Two));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            ScreenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //Compensate for TiledSharp's inability to parse tiles without a gid value
            CleanTmxFiles();

            const int solTextHeight = 460;
            ITexture2D logoTextTexture = AssetManager.MainMenuLogoTexture;
            IRenderable mainMenuTitleSprite = new SpriteAtlas(
                logoTextTexture,
                new Vector2(logoTextTexture.Width, logoTextTexture.Height),
                new Vector2((float) logoTextTexture.Width * solTextHeight / logoTextTexture.Height, solTextHeight)
            );

            MainMenuView mainMenu = new MainMenuView(mainMenuTitleSprite);
            NetworkMenuView networkMenu = new NetworkMenuView(mainMenuTitleSprite);

            InitializeControllers();

            PauseScreenView.Initialize(this);

            GameContext.Initialize(mainMenu, networkMenu);
            InitializeControlMappers(GameContext.P1Team);

            ConnectionManager = new ConnectionManager();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            AssetManager.LoadContent(Content);
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ConnectionManager.Listen();
            ScreenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if (_quitting)
            {
                Exit();
            }

            if (new InputKey(Keys.F10).Pressed)
            {
                UseDefaultResolution();
            }

            if (new InputKey(Keys.F11).Pressed)
            {
                UseBorderlessFullscreen();
            }

            if (new InputKey(Keys.D0).Pressed)
            {
                MusicBox.Pause();
            }

            if (GlobalEventQueue.UpdateEventsEveryFrame())
            {
                ControlMapper activeController = GetControlMapperForPlayer(GameContext.ActivePlayer);
                switch (GameContext.ActivePlayer)
                {
                    case PlayerIndex.One:
                        if (ConnectionManager.ConnectedAsServer)
                        {
                            ControlContext.ListenForInputs(activeController);
                        }
                        else if (ConnectionManager.ConnectedAsClient)
                        {
                            //Do nothing
                        }
                        else
                        {
                            ControlContext.ListenForInputs(activeController);
                        }

                        break;
                    case PlayerIndex.Two:
                        if (ConnectionManager.ConnectedAsClient)
                        {
                            ControlContext.ListenForInputs(activeController);
                        }
                        else if (ConnectionManager.ConnectedAsServer)
                        {
                            //Do nothing
                        }
                        else
                        {
                            ControlContext.ListenForInputs(activeController);
                        }

                        break;
                    case PlayerIndex.Three:

                        if (ConnectionManager.ConnectedAsServer)
                        {
                            //Only allow host to proceed through AI phase
                            ControlContext.ListenForInputs(activeController);
                        }
                        else if (ConnectionManager.ConnectedAsClient)
                        {
                            //Do nothing
                        }
                        else
                        {
                            //Either player can proceed offline
                            ControlContext.ListenForInputs(activeController);
                        }

                        break;
                    case PlayerIndex.Four:
                        ControlContext.ListenForInputs(activeController);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.EULAConfirm:
                    GameContext.UpdateCamera();
                    break;
                case GameContext.GameState.MainMenu:
                    GameContext.UpdateCamera();
                    break;
                case GameContext.GameState.NetworkMenu:
                    break;
                case GameContext.GameState.ArmyDraft:
                    GameContext.UpdateCamera();
                    break;
                case GameContext.GameState.Deployment:
                    GameContext.UpdateCamera();
                    break;
                case GameContext.GameState.MapSelect:
                    GameContext.UpdateCamera();
                    break;
                case GameContext.GameState.PauseScreen:
                    break;
                case GameContext.GameState.InGame:
                    GameContext.GameMapContext.UpdateHoverContextWindows();
                    GameContext.UpdateCamera();
                    break;
                case GameContext.GameState.Codex:
                    break;
                case GameContext.GameState.Credits:
                    break;
                case GameContext.GameState.Results:
                    GameContext.UpdateCamera();
                    break;
                case GameContext.GameState.ItemPreview:
                    break;
                case GameContext.GameState.ControlConfig:
                    GameContext.ControlConfigContext.Update();
                    break;
                case GameContext.GameState.HowToPlay:
                    GameContext.UpdateCamera();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.EULAConfirm:
                    DrawBackgroundWallpaper();
                    DrawMapSelectMap();
                    DrawEULAPrompt();
                    break;
                case GameContext.GameState.MainMenu:
                    DrawBackgroundWallpaper();
                    DrawMapSelectMap();
                    DrawMainMenu();
                    break;
                case GameContext.GameState.NetworkMenu:
                    DrawBackgroundWallpaper();
                    DrawNetworkMenu();
                    break;
                case GameContext.GameState.Deployment:
                    DrawInGameMap();
                    DrawDeploymentHUD();
                    break;
                case GameContext.GameState.ArmyDraft:
                    DrawInGameMap();
                    DrawDraftMenu();
                    break;
                case GameContext.GameState.MapSelect:
                    DrawMapSelectMap();
                    DrawMapSelectHUD();
                    break;
                case GameContext.GameState.PauseScreen:
                    DrawBackgroundWallpaper();
                    DrawPauseMenu();
                    break;
                case GameContext.GameState.InGame:
                    DrawInGameMap();
                    if (GameContext.GameMapContext.CurrentTurnState == GameMapContext.TurnState.UnitActing)
                    {
                        DrawColorEntireScreen(ActionFade);
                    }

                    DrawInGameHUD();
                    break;
                case GameContext.GameState.Codex:
                    DrawBackgroundWallpaper();
                    DrawCodexScreen();
                    break;
                case GameContext.GameState.Results:
                    DrawInGameMap();
                    DrawGameResultsScreen();
                    break;
                case GameContext.GameState.Credits:
                    DrawBackgroundWallpaper();
                    DrawCreditsScreen();
                    break;
                case GameContext.GameState.ItemPreview:
                    DrawInGameMap();
                    DrawColorEntireScreen(ActionFade);
                    DrawInGameHUD();
                    break;
                case GameContext.GameState.ControlConfig:
                    DrawBackgroundWallpaper();
                    DrawControlConfigScreen();
                    break;
                case GameContext.GameState.HowToPlay:
                    DrawBackgroundWallpaper();
                    DrawHowToPlayScreen();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            DrawSystemHUD();
        }

        private void DrawBackgroundWallpaper()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.BackgroundView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawPauseMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            PauseScreenView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawEULAPrompt()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.EULAContext.EULAView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMainMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.MainMenuView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawNetworkMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.NetworkMenuView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMapSelectMap()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null,
                GameContext.MapCamera.CameraMatrix);

            GameContext.MapSelectContext.MapContainer.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawMapSelectHUD()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.MapSelectContext.MapSelectScreenView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawDraftMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.DraftContext.DraftView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawGameResultsScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            GameContext.StatusScreenView.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawCreditsScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            GameContext.CreditsContext.CreditsView.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawHowToPlayScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            GameContext.HowToPlayContext.HowToPlayView.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawColorEntireScreen(Color color)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            spriteBatch.Draw(AssetManager.WhitePixel.MonoGameTexture,
                new Rectangle(0, 0, (int) ScreenSize.X, (int) ScreenSize.Y), color);
            spriteBatch.End();
        }

        private void DrawDeploymentHUD()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.DeploymentContext.DeploymentView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawCodexScreen()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.CodexContext.CodexView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawControlConfigScreen()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GameContext.ControlConfigContext.View.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawInGameMap()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null,
                GameContext.MapCamera.CameraMatrix);
            GameContext.GameMapContext.MapContainer.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawInGameHUD()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            if (GameContext.GameMapContext.CurrentTurnState == GameMapContext.TurnState.UnitActing)
            {
                GameContext.BattleContext.Draw(spriteBatch);
            }
            else
            {
                GameMapContext.GameMapView.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        private void DrawSystemHUD()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            GlobalHudView.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
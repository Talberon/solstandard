using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.InputRemapping;
using SolStandard.Containers.Components.MainMenu;
using SolStandard.Containers.Components.Network;
using SolStandard.Containers.Components.World;
using SolStandard.Containers.Components.World.SubContext.Pause;
using SolStandard.Containers.Scenario;
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
using static System.Reflection.Assembly;

namespace SolStandard
{
    public class GameDriver : Game
    {
        // ReSharper disable once NotAccessedField.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private GraphicsDeviceManager graphics;
        
        public static readonly IFileIO FileIO = new TemporaryFilesIO();

        // ReSharper disable once RedundantDefaultMemberInitializer
        public const bool DeveloperMode = false;

        // ReSharper disable once RedundantDefaultMemberInitializer
        public static bool DebugMode = false;

        //Project Site
        public const string SolStandardUrl = "https://solstandard.talberon.com";

        public static readonly string VersionNumber = GetExecutingAssembly().GetName().Version?.ToString()!;

        //Tile Size of Sprites
        public const int CellSize = 32;
        public const float CellSizeFloat = CellSize;
        public static readonly Vector2 CellSizeVector = new Vector2(CellSizeFloat);

        public static readonly string TmxObjectTypeDefaults =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "Content/TmxMaps/objecttypes.xml");

        private static readonly Color BackgroundColor = new Color(20, 11, 40);
        private static readonly Color ActionFade = new Color(0, 0, 0, 190);
        public static Random Random = new Random();
        public static ConnectionManager ConnectionManager;

        private SpriteBatch spriteBatch;
        private static ControlMapper _blueTeamControlMapper;
        private static ControlMapper _redTeamControlMapper;

        private static bool _quitting;
        public static GameControlParser KeyboardParser;
        public static GameControlParser P1GamepadParser;
        public static GameControlParser P2GamepadParser;
        private IRenderable mainMenuLogo;

        //Resolution
        public static Vector2 ScreenSize { get; private set; }
        public static Vector2 RenderResolution { get; private set; }
        public static RectangleF VirtualBounds => new RectangleF(Point2.Zero, VirtualResolution);
        public static Point VirtualResolution { get; private set; }

        public static Vector2 CenterScreen =>
            new Vector2((float) VirtualResolution.X / 2, (float) VirtualResolution.Y / 2);

        public static BoxingViewportAdapter BoxingViewportAdapter { get; set; }

        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            UseDefaultResolution();
//            UseBorderlessFullscreen();
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            IsFixedTimeStep = true;
            Window.AllowUserResizing = true;

            VirtualResolution = new Point(1600, 900);
            var windowResolution = new Point(1280, 720);

            graphics.PreferredBackBufferWidth = windowResolution.X;
            graphics.PreferredBackBufferHeight = windowResolution.Y;

            BoxingViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, VirtualResolution.X,
                VirtualResolution.Y);
        }
        
        public void ToggleFullscreen()
        {
            graphics.HardwareModeSwitch = false;
            graphics.ToggleFullScreen();
        }

        private void UseDefaultResolution()
        {
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();
            Window.Position = new Point(0, 50);
            Window.IsBorderless = false;
            Window.AllowUserResizing = true;
        }
        
        public static void NewGame(string mapName, Scenario scenario, Team firstTeam)
        {
            GlobalContext.StartGame(mapName, scenario, firstTeam);
        }

        public static void HostGame()
        {
            //Start Server
            string serverIP = ConnectionManager.StartServer();
            GlobalContext.NetworkHUD.UpdateStatus(serverIP, true, serverIP != null);
            GlobalContext.NetworkHUD.GenerateHostMenu();
            GlobalContext.NetworkHUD.RemoveDialMenu();
            GlobalContext.CurrentGameState = GlobalContext.GameState.NetworkMenu;
        }

        public static void JoinGame(string serverIPAddress = "127.0.0.1")
        {
            //Start Client
            ConnectionManager.StartClient(serverIPAddress, ConnectionManager.NetworkPort);
            GlobalContext.NetworkHUD.UpdateStatus(serverIPAddress, false);
            GlobalContext.NetworkHUD.GenerateDialMenu();
            GlobalContext.NetworkHUD.RemoveHostMenu();
            GlobalContext.CurrentGameState = GlobalContext.GameState.NetworkMenu;
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
            string tmxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, @"Content/TmxMaps/");
            var tmxName = new Regex("([\\w])+.tmx");
            foreach (string tmxFile in Directory.GetFiles(tmxPath).Where(filename => tmxName.IsMatch(filename)))
            {
                string text = File.ReadAllText(tmxFile);
                text = text.Replace("tile/", "tile gid=\"0\"/");
                File.WriteAllText(tmxFile, text);
            }
        }

        public static ControlMapper GetControlMapperForPlayer(PlayerIndex playerIndex)
        {
            return playerIndex switch
            {
                PlayerIndex.One => ((GlobalContext.P1Team == Team.Blue)
                    ? _blueTeamControlMapper
                    : _redTeamControlMapper),
                PlayerIndex.Two => ((GlobalContext.P2Team == Team.Blue)
                    ? _blueTeamControlMapper
                    : _redTeamControlMapper),
                _ => GetControlMapperForPlayer(PlayerIndex.One)
            };
        }

        private static void InitializeControllers()
        {
            var loadedKeyboardConfig =
                FileIO.Load<IController>(ControlConfigContext.KeyboardConfigFileName);
            KeyboardParser = new GameControlParser(loadedKeyboardConfig ?? new KeyboardController());

            var loadedP1GamepadConfig =
                FileIO.Load<IController>(ControlConfigContext.P1GamepadConfigFileName);
            P1GamepadParser = new GameControlParser(loadedP1GamepadConfig ?? new GamepadController(PlayerIndex.One));

            var loadedP2GamepadConfig =
                FileIO.Load<IController>(ControlConfigContext.P2GamepadConfigFileName);
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

            RenderResolution = new Vector2(BoxingViewportAdapter.VirtualWidth, BoxingViewportAdapter.VirtualHeight);
            ScreenSize = VirtualResolution.ToVector2();

            //Compensate for TiledSharp's inability to parse tiles without a gid value
            CleanTmxFiles();

            const int solTextHeight = 250;
            ITexture2D logoTextTexture = AssetManager.MainMenuLogoTexture;
            mainMenuLogo = new SpriteAtlas(
                logoTextTexture,
                new Vector2(logoTextTexture.Width, logoTextTexture.Height),
                new Vector2((float) logoTextTexture.Width * solTextHeight / logoTextTexture.Height, solTextHeight)
            );

            InitializeControllers();

            var mainMenu = new MainMenuHUD(mainMenuLogo);
            var networkMenu = new NetworkHUD(mainMenuLogo);

            GlobalContext.Initialize(mainMenu, networkMenu);
            
            PauseScreenUtils.Initialize(this);
            
            InitializeControlMappers(GlobalContext.P1Team);

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

            if (_quitting)
            {
                Exit();
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (DeveloperMode)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D1)) DebugMode = true;
                if (Keyboard.GetState().IsKeyDown(Keys.D2)) DebugMode = false;

                if (new InputKey(Keys.D0).Pressed)
                {
                    MusicBox.Pause();
                }
            }

            if (new InputKey(Keys.F11).Pressed)
            {
                ToggleFullscreen();
            }

            if (GlobalContext.CurrentGameState == GlobalContext.GameState.SplashScreen)
            {
                GlobalContext.SplashScreenContext.Update(gameTime);
            }

            HandleInput();
            UpdateCamera();
            GlobalAsyncActions.Update(gameTime);

            base.Update(gameTime);
        }

        private static void UpdateCamera()
        {
            switch (GlobalContext.CurrentGameState)
            {
                case GlobalContext.GameState.SplashScreen:
                    return;
                case GlobalContext.GameState.EULAConfirm:
                    GlobalContext.UpdateCamera();
                    break;
                case GlobalContext.GameState.MainMenu:
                    GlobalContext.UpdateCamera();
                    break;
                case GlobalContext.GameState.NetworkMenu:
                    break;
                case GlobalContext.GameState.ArmyDraft:
                    GlobalContext.UpdateCamera();
                    break;
                case GlobalContext.GameState.Deployment:
                    GlobalContext.UpdateCamera();
                    break;
                case GlobalContext.GameState.MapSelect:
                    GlobalContext.UpdateCamera();
                    break;
                case GlobalContext.GameState.PauseScreen:
                    break;
                case GlobalContext.GameState.InGame:
                    GlobalContext.WorldContext.UpdateHoverContextWindows();
                    GlobalContext.UpdateCamera();
                    break;
                case GlobalContext.GameState.Codex:
                    break;
                case GlobalContext.GameState.Credits:
                    break;
                case GlobalContext.GameState.Results:
                    GlobalContext.UpdateCamera();
                    break;
                case GlobalContext.GameState.ItemPreview:
                    break;
                case GlobalContext.GameState.ControlConfig:
                    GlobalContext.ControlConfigContext.Update();
                    break;
                case GlobalContext.GameState.HowToPlay:
                    GlobalContext.UpdateCamera();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void HandleInput()
        {
            if (GlobalEventQueue.UpdateEventsEveryFrame())
            {
                ControlMapper activeController = GetControlMapperForPlayer(GlobalContext.ActivePlayer);
                switch (GlobalContext.ActivePlayer)
                {
                    case PlayerIndex.One:
                        if (ConnectionManager.ConnectedAsServer)
                        {
                            InputListener.ListenForInputs(activeController);
                        }
                        else if (ConnectionManager.ConnectedAsClient)
                        {
                            //Do nothing
                        }
                        else
                        {
                            InputListener.ListenForInputs(activeController);
                        }

                        break;
                    case PlayerIndex.Two:
                        if (ConnectionManager.ConnectedAsClient)
                        {
                            InputListener.ListenForInputs(activeController);
                        }
                        else if (ConnectionManager.ConnectedAsServer)
                        {
                            //Do nothing
                        }
                        else
                        {
                            InputListener.ListenForInputs(activeController);
                        }

                        break;
                    case PlayerIndex.Three:

                        if (ConnectionManager.ConnectedAsServer)
                        {
                            //Only allow host to proceed through AI phase
                            InputListener.ListenForInputs(activeController);
                        }
                        else if (ConnectionManager.ConnectedAsClient)
                        {
                            //Do nothing
                        }
                        else
                        {
                            //Either player can proceed offline
                            InputListener.ListenForInputs(activeController);
                        }

                        break;
                    case PlayerIndex.Four:
                        InputListener.ListenForInputs(activeController);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            switch (GlobalContext.CurrentGameState)
            {
                case GlobalContext.GameState.SplashScreen:
                    DrawSplashScreen();
                    break;
                case GlobalContext.GameState.EULAConfirm:
                    DrawBackgroundWallpaper();
                    DrawMapSelectMap();
                    DrawEULAPrompt();
                    break;
                case GlobalContext.GameState.MainMenu:
                    DrawBackgroundWallpaper();
                    DrawMapSelectMap();
                    DrawMainMenu();
                    break;
                case GlobalContext.GameState.NetworkMenu:
                    DrawBackgroundWallpaper();
                    DrawNetworkMenu();
                    break;
                case GlobalContext.GameState.Deployment:
                    DrawInGameMap();
                    DrawDeploymentHUD();
                    break;
                case GlobalContext.GameState.ArmyDraft:
                    DrawInGameMap();
                    DrawDraftMenu();
                    break;
                case GlobalContext.GameState.MapSelect:
                    DrawMapSelectMap();
                    DrawMapSelectHUD();
                    break;
                case GlobalContext.GameState.PauseScreen:
                    DrawBackgroundWallpaper();
                    DrawPauseMenu();
                    break;
                case GlobalContext.GameState.InGame:
                    DrawInGameMap();
                    if (GlobalContext.WorldContext.CurrentTurnState == WorldContext.TurnState.UnitActing)
                    {
                        DrawColorEntireScreen(ActionFade);
                    }

                    DrawInGameHUD();
                    break;
                case GlobalContext.GameState.Codex:
                    DrawBackgroundWallpaper();
                    DrawCodexScreen();
                    break;
                case GlobalContext.GameState.Results:
                    DrawInGameMap();
                    DrawGameResultsScreen();
                    break;
                case GlobalContext.GameState.Credits:
                    DrawBackgroundWallpaper();
                    DrawCreditsScreen();
                    break;
                case GlobalContext.GameState.ItemPreview:
                    DrawInGameMap();
                    DrawColorEntireScreen(ActionFade);
                    DrawInGameHUD();
                    break;
                case GlobalContext.GameState.ControlConfig:
                    DrawBackgroundWallpaper();
                    DrawControlConfigScreen();
                    break;
                case GlobalContext.GameState.HowToPlay:
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
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.StaticBackgroundView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawPauseMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            PauseScreenUtils.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawSplashScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.SplashScreenContext.SplashScreenHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawEULAPrompt()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.EULAContext.EULAHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMainMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.MainMenuHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawNetworkMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.NetworkHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMapSelectMap()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GlobalContext.MapCamera.CameraMatrix);
            GlobalContext.MapSelectContext.MapContainer.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMapSelectHUD()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.MapSelectContext.MapSelectHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawDraftMenu()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.DraftContext.DraftHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawGameResultsScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.StatusScreenHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawCreditsScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.CreditsContext.CreditsHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawHowToPlayScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.HowToPlayContext.HowToPlayHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawColorEntireScreen(Color color)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            spriteBatch.Draw(AssetManager.WhitePixel.MonoGameTexture,
                new Rectangle(0, 0, (int) ScreenSize.X, (int) ScreenSize.Y), color);
            spriteBatch.End();
        }

        private void DrawDeploymentHUD()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.DeploymentContext.DeploymentHUD.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawCodexScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.CodexContext.CodexView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawControlConfigScreen()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalContext.ControlConfigContext.View.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawInGameMap()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GlobalContext.MapCamera.CameraMatrix);
            GlobalContext.WorldContext.MapContainer.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawInGameHUD()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));

            if (GlobalContext.WorldContext.CurrentTurnState == WorldContext.TurnState.UnitActing)
            {
                GlobalContext.CombatPhase.Draw(spriteBatch);
            }
            else
            {
                WorldContext.WorldHUD.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        private void DrawSystemHUD()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, samplerState: SamplerState.PointClamp,
                transformMatrix: GetRenderResolutionForVirtualResolution(VirtualResolution));
            GlobalHUDUtils.Draw(spriteBatch);
            spriteBatch.End();
        }

        private Matrix GetRenderResolutionForVirtualResolution(Point virtualResolution)
        {
            (int x, int y) = virtualResolution;
            return Matrix.CreateScale(
                (float) GraphicsDevice.Viewport.Width / x,
                (float) GraphicsDevice.Viewport.Height / y,
                1f
            );
        }
    }
}
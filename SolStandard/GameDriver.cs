﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Gamepad;
using SolStandard.Utility.Buttons.KeyboardInput;
using SolStandard.Utility.Events;
using SolStandard.Utility.Monogame;
using SolStandard.Utility.Network;
using Keys = Microsoft.Xna.Framework.Input.Keys;

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

        //Project Site
        public const string SolStandardUrl = "https://talberon.github.io/solstandard";

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

        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            UseDefaultResolution();
//            UseBorderlessFullscreen();
            Content.RootDirectory = "Content";
        }

        private void UseDefaultResolution()
        {
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //FIXME HACK move the window away from the top of the screen
            Window.Position = new Point(0, 50);
            Window.IsBorderless = false;
            graphics.ApplyChanges();
        }

        // ReSharper disable once UnusedMember.Local

        private void UseBorderlessFullscreen()
        {
            Screen currentScreen = Screen.FromPoint(new System.Drawing.Point(Window.Position.X, Window.Position.Y));
            graphics.PreferredBackBufferWidth = currentScreen.Bounds.Width;
            graphics.PreferredBackBufferHeight = currentScreen.Bounds.Height;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Window.Position = new Point(0, 0);
            Window.IsBorderless = true;
            graphics.ApplyChanges();
        }


        /// <summary>
        /// Starts a new game by generating a new map
        /// </summary>
        public static void NewGame(string mapName, Scenario scenario)
        {
            GameContext.StartGame(mapName, scenario);
        }

        public static void HostGame()
        {
            //Start Server
            IPAddress serverIP = ConnectionManager.StartServer();
            string serverIPAddress =
                (serverIP != null) ? serverIP.ToString() : "Could not obtain external IP automatically.";
            GameContext.NetworkMenuView.UpdateStatus(serverIPAddress, true, serverIP != null);
            GameContext.NetworkMenuView.RemoveDialMenu();
            GameContext.CurrentGameState = GameContext.GameState.NetworkMenu;
        }

        public static void JoinGame(string serverIPAddress = "127.0.0.1")
        {
            //Start Client
            ConnectionManager.StartClient(serverIPAddress, ConnectionManager.NetworkPort);
            GameContext.NetworkMenuView.UpdateStatus(serverIPAddress, false);
            GameContext.NetworkMenuView.GenerateDialMenu();
            GameContext.CurrentGameState = GameContext.GameState.NetworkMenu;
        }

        public static bool ConnectedAsServer => ConnectionManager.ConnectedAsServer;

        public static bool ConnectedAsClient => ConnectionManager.ConnectedAsClient;

        public static void SetControllerConfig(Team playerOneTeam)
        {
            GameControlParser keyboardParser = new GameControlParser(new KeyboardController());
            GameControlParser p1GamepadParser = new GameControlParser(new GamepadController(PlayerIndex.One));
            GameControlParser p2GamepadParser = new GameControlParser(new GamepadController(PlayerIndex.Two));

            switch (playerOneTeam)
            {
                case Team.Blue:
                    _blueTeamControlMapper = new MultiControlParser(keyboardParser, p1GamepadParser);
                    _redTeamControlMapper = new MultiControlParser(keyboardParser, p2GamepadParser);
                    break;
                case Team.Red:
                    _redTeamControlMapper = new MultiControlParser(keyboardParser, p1GamepadParser);
                    _blueTeamControlMapper = new MultiControlParser(keyboardParser, p2GamepadParser);
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
            switch (playerIndex)
            {
                case PlayerIndex.One:
                    return (GameContext.P1Team == Team.Blue) ? _blueTeamControlMapper : _redTeamControlMapper;
                case PlayerIndex.Two:
                    return (GameContext.P2Team == Team.Blue) ? _blueTeamControlMapper : _redTeamControlMapper;
                default:
                    return GetControlMapperForPlayer(PlayerIndex.One);
            }
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

            SpriteAtlas mainMenuTitleSprite = new SpriteAtlas(AssetManager.MainMenuLogoTexture,
                new Vector2(AssetManager.MainMenuLogoTexture.Width, AssetManager.MainMenuLogoTexture.Height));
            AnimatedSpriteSheet mainMenuLogoSpriteSheet =
                new AnimatedSpriteSheet(AssetManager.MainMenuSunTexture, AssetManager.MainMenuSunTexture.Height, 5,
                    false);
            SpriteAtlas mainMenuBackgroundSprite = new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                new Vector2(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));

            MainMenuView mainMenu =
                new MainMenuView(mainMenuTitleSprite, mainMenuLogoSpriteSheet, mainMenuBackgroundSprite);
            NetworkMenuView networkMenu =
                new NetworkMenuView(mainMenuTitleSprite, mainMenuLogoSpriteSheet, mainMenuBackgroundSprite);

            GameContext.Initialize(mainMenu, networkMenu);
            SetControllerConfig(GameContext.P1Team);

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

            if (new InputKey(Keys.F10).Pressed)
            {
                UseDefaultResolution();
                ScreenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }

            if (new InputKey(Keys.F11).Pressed)
            {
                UseBorderlessFullscreen();
                ScreenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }


            if (Keyboard.GetState().IsKeyDown(Keys.D0))
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
                case GameContext.GameState.MainMenu:
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
                    break;
                case GameContext.GameState.ItemPreview:
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
                case GameContext.GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameContext.GameState.NetworkMenu:
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
                    DrawInGameMap();
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
                    DrawCodexScreen();
                    break;
                case GameContext.GameState.Results:
                    DrawGameResultsScreen();
                    break;
                case GameContext.GameState.Credits:
                    DrawCreditsScreen();
                    break;
                case GameContext.GameState.ItemPreview:
                    DrawInGameMap();
                    DrawColorEntireScreen(ActionFade);
                    DrawInGameHUD();
                    break;
                default:
                    base.Draw(gameTime);
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawPauseMenu()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);
            GameContext.GameMapContext.PauseScreenView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMainMenu()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);
            GameContext.MainMenuView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawNetworkMenu()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);
            GameContext.NetworkMenuView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMapSelectMap()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, GameContext.MapCamera.CameraMatrix);

            GameContext.MapSelectContext.MapContainer.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawMapSelectHUD()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);
            GameContext.MapSelectContext.MapSelectScreenView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawDraftMenu()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);
            GameContext.DraftContext.DraftView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawGameResultsScreen()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);

            GameContext.StatusScreenView.Draw(spriteBatch);

            spriteBatch.End();
        }


        private void DrawCreditsScreen()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);

            GameContext.CreditsContext.CreditsView.Draw(spriteBatch);

            spriteBatch.End();
        }


        private void DrawColorEntireScreen(Color color)
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null,
                SamplerState.PointClamp
            );
            spriteBatch.Draw(AssetManager.WhitePixel.MonoGameTexture,
                new Rectangle(0, 0, (int) ScreenSize.X, (int) ScreenSize.Y), color);
            spriteBatch.End();
        }

        private void DrawDeploymentHUD()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);
            GameContext.DeploymentContext.DeploymentView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawCodexScreen()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);
            GameContext.CodexContext.CodexView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawInGameMap()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, GameContext.MapCamera.CameraMatrix);
            GameContext.GameMapContext.MapContainer.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawInGameHUD()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp);

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
    }
}
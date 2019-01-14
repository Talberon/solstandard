using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Containers.View;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.KeyboardInput;
using SolStandard.Utility.Buttons.Network;
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
        private GraphicsDeviceManager graphics;

        //Tile Size of Sprites
        public const int CellSize = 32;
        public const string TmxObjectTypeDefaults = "Content/TmxMaps/objecttypes.xml";

        public static Random Random = new Random();
        public static Vector2 ScreenSize { get; private set; }
        private static ConnectionManager _connectionManager;
        
        private SpriteBatch spriteBatch;
        private ControlMapper blueTeamControlMapper;
        private ControlMapper redTeamControlMapper;
        private NetworkController networkController;
        private NetworkController lastNetworkControlSent;

        private static bool _quitting;


        public GameDriver()
        {
            UseDebugResolution();
            //UseBorderlessFullscreen();
            Content.RootDirectory = "Content";
        }

        private void UseDebugResolution()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            //FIXME HACK move the window away from the top of the screen
            Window.Position = new Point(0, 50);
            Window.IsBorderless = false;
        }

        // ReSharper disable once UnusedMember.Local
        private void UseBorderlessFullscreen()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Screen.PrimaryScreen.Bounds.Width,
                PreferredBackBufferHeight = Screen.PrimaryScreen.Bounds.Height,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            Window.Position = new Point(0, 0);
            Window.IsBorderless = true;
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
            _connectionManager.StartServer();
        }

        public static void JoinGame()
        {
            //Start Client
            //TODO Update this to take an argument instead of a hard-coded IP
            _connectionManager.StartClient("127.0.0.1", 4444);
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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            networkController = new NetworkController();

            ScreenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //Compensate for TiledSharp's inability to parse tiles without a gid value
            CleanTmxFiles();

            SpriteAtlas mainMenuTitleSprite = new SpriteAtlas(AssetManager.MainMenuLogoTexture,
                new Vector2(AssetManager.MainMenuLogoTexture.Width, AssetManager.MainMenuLogoTexture.Height));
            AnimatedSpriteSheet mainMenuLogoSpriteSheet =
                new AnimatedSpriteSheet(AssetManager.MainMenuSunTexture, AssetManager.MainMenuSunTexture.Height, 5,
                    false);
            SpriteAtlas mainMenuBackgroundSprite = new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height), ScreenSize);

            blueTeamControlMapper = new GameControlParser(new KeyboardController());
            redTeamControlMapper = new GameControlParser(new KeyboardController());

            GameContext.Initialize(new MainMenuView(mainMenuTitleSprite, mainMenuLogoSpriteSheet,
                mainMenuBackgroundSprite));
            MusicBox.PlayLoop(AssetManager.MusicTracks.Find(track => track.Name.Contains("MapSelect")), 0.3f);

            _connectionManager = new ConnectionManager();
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
            _connectionManager.Listen();

            if (_quitting)
            {
                Exit();
            }

            //TODO Remove Debug actions
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                GameContext.CurrentGameState = GameContext.GameState.MainMenu;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                GameContext.CurrentGameState = GameContext.GameState.InGame;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                GameContext.CurrentGameState = GameContext.GameState.Results;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                GameContext.CurrentGameState = GameContext.GameState.PauseScreen;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D0))
            {
                MusicBox.Pause();
            }

            //Set the controller based on the active team
            if (GameContext.CurrentGameState >= GameContext.GameState.InGame)
            {
                switch (GameContext.ActiveUnit.Team)
                {
                    case Team.Blue:
                        GameContext.ActivePlayer = PlayerIndex.One;
                        break;
                    case Team.Red:
                        GameContext.ActivePlayer = PlayerIndex.Two;
                        break;
                    case Team.Creep:
                        GameContext.ActivePlayer = PlayerIndex.Three;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (GlobalEventQueue.UpdateEventsEveryFrame())
            {
                switch (GameContext.ActivePlayer)
                {
                    case PlayerIndex.One:
                        if (_connectionManager.ConnectedAsServer)
                        {
                            networkController = ControlContext.ListenForInputs(blueTeamControlMapper);
                            SendServerControls();
                        }
                        else if (_connectionManager.ConnectedAsClient)
                        {
                            //Do nothing
                        }
                        else
                        {
                            ControlContext.ListenForInputs(blueTeamControlMapper);
                        }

                        break;
                    case PlayerIndex.Two:
                        if (_connectionManager.ConnectedAsClient)
                        {
                            networkController = ControlContext.ListenForInputs(redTeamControlMapper);
                            SendClientControls();
                        }
                        else if (_connectionManager.ConnectedAsServer)
                        {
                            //Do nothing
                        }
                        else
                        {
                            ControlContext.ListenForInputs(redTeamControlMapper);
                        }

                        break;
                    case PlayerIndex.Three:
                        networkController = ControlContext.ListenForInputs(blueTeamControlMapper);

                        if (_connectionManager.ConnectedAsServer)
                        {
                            //Only allow host to proceed through AI phase
                            SendServerControls();
                        }
                        else
                        {
                            //Either player can proceed offline
                            ControlContext.ListenForInputs(redTeamControlMapper);
                        }

                        break;
                    case PlayerIndex.Four:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.MainMenu:
                    break;
                case GameContext.GameState.ModeSelect:
                    break;
                case GameContext.GameState.ArmyDraft:
                    break;
                case GameContext.GameState.MapSelect:
                    GameContext.UpdateCamera();
                    GameContext.MapSelectContext.HoverOverEntity();

                    break;
                case GameContext.GameState.PauseScreen:
                    break;
                case GameContext.GameState.InGame:
                    GameContext.UpdateCamera();
                    MapSlice hoverTiles = GameContext.GameMapContext.MapContainer.GetMapSliceAtCursor();
                    GameContext.GameMapContext.UpdateHoverContextWindows(hoverTiles);
                    break;
                case GameContext.GameState.Results:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.Update(gameTime);
        }

        private void SendServerControls()
        {
            if (lastNetworkControlSent != null && lastNetworkControlSent.Equals(networkController)) return;

            //Send Message From Server to Client
            _connectionManager.SendTextMessageAsServer("MESSAGE FROM SERVER TO CLIENT :^)");
            _connectionManager.SendControlMessageAsServer(networkController);
            lastNetworkControlSent = networkController;
        }

        private void SendClientControls()
        {
            if (lastNetworkControlSent != null && lastNetworkControlSent.Equals(networkController)) return;

            //Send message from client to server
            _connectionManager.SendTextMessageAsClient("MESSAGE FROM CLIENT TO SERVER :D");
            _connectionManager.SendControlMessageAsClient(networkController);
            lastNetworkControlSent = networkController;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(38, 43, 64));


            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameContext.GameState.ModeSelect:
                    break;
                case GameContext.GameState.ArmyDraft:
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
                        DrawColorEntireScreen(new Color(0, 0, 0, 190));
                    }

                    DrawInGameHUD();
                    break;
                case GameContext.GameState.Results:
                    DrawGameResultsScreen();
                    break;
                default:
                    base.Draw(gameTime);
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawPauseMenu()
        {
            //Render Main Menu
            spriteBatch.Begin(
                SpriteSortMode
                    .Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);
            GameContext.GameMapContext.PauseScreenView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMainMenu()
        {
            //Render Main Menu
            spriteBatch.Begin(
                SpriteSortMode
                    .Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);
            GameContext.MainMenuView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMapSelectMap()
        {
            //MAP LAYER
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, GameContext.MapCamera.CameraMatrix);

            GameContext.MapSelectContext.MapContainer.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawMapSelectHUD()
        {
            //HUD
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);
            GameContext.MapSelectContext.MapSelectScreenView.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawGameResultsScreen()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);

            GameContext.StatusScreenView.Draw(spriteBatch);

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
                null, SamplerState.PointClamp, null, null, null, null);

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
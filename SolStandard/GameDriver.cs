using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Events;
using SolStandard.Utility.Monogame;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace SolStandard
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameDriver : Game
    {
        //Tile Size of Sprites
        public const int CellSize = 32;
        public const string TmxObjectTypeDefaults = "Content/TmxMaps/objecttypes.xml";

        public static readonly Random Random = new Random();
        public static Vector2 ScreenSize { get; private set; }
        public static List<MapInfo> AvailableMaps;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameControlMapper p1ControlMapper;
        private GameControlMapper p2ControlMapper;

        private static MapCamera _mapCamera;
        private static GameContext _gameContext;
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
                PreferredBackBufferHeight = 900
            };

            //FIXME HACK move the window away from the top of the screen
            Window.Position = new Point(0, 50);
            Window.IsBorderless = false;
        }

        private void UseBorderlessFullscreen()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Screen.PrimaryScreen.Bounds.Width,
                PreferredBackBufferHeight = Screen.PrimaryScreen.Bounds.Height
            };

            Window.Position = new Point(0, 0);
            Window.IsBorderless = true;
        }


        /// <summary>
        /// Starts a new game by generating a new map
        /// </summary>
        public static void NewGame(string mapName)
        {
            string mapPath = "Content/TmxMaps/" + mapName;
            _gameContext.StartGame(mapPath);
        }

        public static void QuitGame()
        {
            _quitting = true;
        }

        private static void CleanTmxFiles(IEnumerable<MapInfo> mapList)
        {
            foreach (MapInfo mapInfo in mapList)
            {
                string filePath = "Content/TmxMaps/" + mapInfo.FileName;

                string text = File.ReadAllText(filePath);
                text = text.Replace("tile/", "tile gid=\"0\"/");
                File.WriteAllText(filePath, text);
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

            _mapCamera = new MapCamera(5, 0.05f);

            ITexture2D mapPreviewGrass3 =
                AssetManager.MapPreviewTextures.Find(texture => texture.Name.Contains("MapPreviews/Grass_03"));
            ITexture2D mapPreviewGrass4 =
                AssetManager.MapPreviewTextures.Find(texture => texture.Name.Contains("MapPreviews/Grass_04"));

            AvailableMaps = new List<MapInfo>
            {
                new MapInfo("MapSelect", "Map_Select_02.tmx", new RenderBlank()),

                new MapInfo("Old World", "Experimenting_01.tmx",
                    new SpriteAtlas(mapPreviewGrass3, new Vector2(mapPreviewGrass3.Width, mapPreviewGrass3.Height), 1)),
                new MapInfo("Beachhead", "Experimenting_02.tmx",
                    new SpriteAtlas(mapPreviewGrass4, new Vector2(mapPreviewGrass4.Width, mapPreviewGrass4.Height), 1)),
                new MapInfo("Dungeon", "Experimenting_03.tmx", new RenderBlank()),
                new MapInfo("Debug", "Debug_01.tmx", new RenderBlank()),
                new MapInfo("Nova Fields", "Experimenting_04.tmx", new RenderBlank()),
                new MapInfo("Scotia Hill", "Experimenting_05.tmx", new RenderBlank())
            };

            //Compensate for TiledSharp's inability to parse tiles without a gid value
            CleanTmxFiles(AvailableMaps);

            SpriteAtlas mainMenuTitleSprite = new SpriteAtlas(AssetManager.MainMenuLogoTexture,
                new Vector2(AssetManager.MainMenuLogoTexture.Width, AssetManager.MainMenuLogoTexture.Height), 1);
            AnimatedSprite mainMenuLogoSprite =
                new AnimatedSprite(AssetManager.MainMenuSunTexture, AssetManager.MainMenuSunTexture.Height, 5, false);
            SpriteAtlas mainMenuBackgroundSprite = new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                ScreenSize, 1);

            p1ControlMapper = new GameControlMapper(PlayerIndex.One);
            p2ControlMapper = new GameControlMapper(PlayerIndex.Two);

            GameContext.ActivePlayer = PlayerIndex.One;

            _gameContext = new GameContext(
                new MainMenuUI(mainMenuTitleSprite, mainMenuLogoSprite, mainMenuBackgroundSprite)
            );

            MapCamera.CenterCameraToCursor();

            MusicBox.PlayLoop(AssetManager.MusicTracks.Find(track => track.Name.Contains("MapSelect")), 0.3f);
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
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_quitting)
            {
                Exit();
            }

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

            if (Keyboard.GetState().IsKeyDown(Keys.D0))
            {
                MusicBox.Pause();
            }

            //Set the controller based on the active team
            if (GameContext.CurrentGameState >= GameContext.GameState.InGame)
            {
                switch (GameContext.ActiveUnit.Team)
                {
                    case Team.Red:
                        GameContext.ActivePlayer = PlayerIndex.One;
                        break;
                    case Team.Blue:
                        GameContext.ActivePlayer = PlayerIndex.Two;
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
                        ControlContext.ListenForInputs(_gameContext, p1ControlMapper, _mapCamera,
                            MapContainer.MapCursor);
                        break;
                    case PlayerIndex.Two:
                        ControlContext.ListenForInputs(_gameContext, p2ControlMapper, _mapCamera,
                            MapContainer.MapCursor);
                        break;
                    case PlayerIndex.Three:
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
                    _mapCamera.UpdateEveryFrame();
                    _gameContext.UpdateCamera(_mapCamera);
                    GameContext.MapSelectContext.HoverOverEntity();

                    break;
                case GameContext.GameState.PauseScreen:
                    break;
                case GameContext.GameState.InGame:
                    _mapCamera.UpdateEveryFrame();
                    _gameContext.UpdateCamera(_mapCamera);
                    MapSlice hoverTiles = MapContainer.GetMapSliceAtCursor();
                    _gameContext.MapContext.UpdateHoverContextWindows(hoverTiles);
                    break;
                case GameContext.GameState.Results:
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
                    break;
                case GameContext.GameState.InGame:
                    DrawInGameMap();
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

        private void DrawMainMenu()
        {
            //Render Main Menu
            spriteBatch.Begin(
                SpriteSortMode
                    .Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);
            _gameContext.MainMenuUI.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMapSelectMap()
        {
            //MAP LAYER
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, MapCamera.CameraMatrix);

            GameContext.MapSelectContext.MapContainer.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawMapSelectHUD()
        {
            //HUD
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);
            GameContext.MapSelectContext.SelectMapUI.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawGameResultsScreen()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);

            _gameContext.ResultsUI.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawInGameMap()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, MapCamera.CameraMatrix);
            _gameContext.MapContext.MapContainer.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawInGameHUD()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //UseAction deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);

            if (_gameContext.MapContext.CurrentTurnState == MapContext.TurnState.UnitActing)
            {
                _gameContext.BattleContext.Draw(spriteBatch);
            }
            else
            {
                _gameContext.MapContext.GameMapUI.Draw(spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
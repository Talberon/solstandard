using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Monogame;

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

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameControlMapper p1ControlMapper;
        private GameControlMapper p2ControlMapper;

        private static MapCamera _mapCamera;
        private static GameContext _gameContext;
        private static bool _quitting;


        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900
            };

            //FIXME HACK Move the window away from the top-left corner
            Window.Position = new Point(0, 50);

            Content.RootDirectory = "Content";
        }


        /// <summary>
        /// Starts a new game by generating a new map
        /// </summary>
        public static void NewGame(string mapName)
        {
            
            string mapPath = "Content/TmxMaps/" + mapName;
            _gameContext.StartGame(mapPath, _mapCamera);
        }

        public static void QuitGame()
        {
            _quitting = true;
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

            AvailableMaps = new List<MapInfo>
            {
                new MapInfo("New World", "Experimenting_01.tmx",
                    new SpriteAtlas(mapPreviewGrass3, new Vector2(mapPreviewGrass3.Width, mapPreviewGrass3.Height), 1))
            };

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

            _mapCamera.CenterCameraToCursor();

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

            switch (GameContext.ActivePlayer)
            {
                case PlayerIndex.One:
                    ControlContext.ListenForInputs(_gameContext, p1ControlMapper, _mapCamera, MapContainer.MapCursor);
                    break;
                case PlayerIndex.Two:
                    ControlContext.ListenForInputs(_gameContext, p2ControlMapper, _mapCamera, MapContainer.MapCursor);
                    break;
                case PlayerIndex.Three:
                    break;
                case PlayerIndex.Four:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
                    _gameContext.MapContext.UpdateUnitPortraitWindows(hoverTiles);
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
                    //Render Main Menu
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, null);
                    _gameContext.MainMenuUI.Draw(spriteBatch);
                    spriteBatch.End();
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

        private void DrawMapSelectMap()
        {
//MAP LAYER
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, _mapCamera.CameraMatrix);

            GameContext.MapSelectContext.MapContainer.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawMapSelectHUD()
        {
//HUD
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);
            GameContext.MapSelectContext.SelectMapUI.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawGameResultsScreen()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);

            _gameContext.ResultsUI.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawInGameMap()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, _mapCamera.CameraMatrix);
            _gameContext.MapContext.MapContainer.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawInGameHUD()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
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
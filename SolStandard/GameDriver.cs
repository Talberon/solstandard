using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Load;
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
        public static Dictionary<string, string> AvailableMaps;

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameControlMapper p1ControlMapper;
        private GameControlMapper p2ControlMapper;

        private static MapCamera _mapCamera;
        private static GameContext _gameContext;
        private static bool _quitting;

        private static List<ITexture2D> WindowTextures { get; set; }
        private static List<ITexture2D> TerrainTextures { get; set; }
        private static List<ITexture2D> GuiTextures { get; set; }
        private static ITexture2D MainMenuLogoTexture { get; set; }

        public static ISpriteFont WindowFont { get; private set; }
        public static ISpriteFont MapFont { get; private set; }
        public static ISpriteFont ResultsFont { get; private set; }
        public static ISpriteFont HeaderFont { get; private set; }
        public static ISpriteFont MainMenuFont { get; private set; }

        public static ITexture2D ActionTiles { get; private set; }
        public static ITexture2D WhitePixel { get; private set; }
        public static ITexture2D WhiteGrid { get; private set; }
        public static ITexture2D DiceTexture { get; private set; }
        public static ITexture2D StatIcons { get; private set; }

        public static List<ITexture2D> UnitSprites { get; private set; }
        public static List<ITexture2D> LargePortraitTextures { get; private set; }
        public static List<ITexture2D> MediumPortraitTextures { get; private set; }
        public static List<ITexture2D> SmallPortraitTextures { get; private set; }

        public static ITexture2D WorldTileSetTexture
        {
            get { return TerrainTextures.Find(texture => texture.Name.Contains("Map/Tiles/WorldTileSet")); }
        }

        public static ITexture2D TerrainTexture
        {
            get { return TerrainTextures.Find(texture => texture.Name.Contains("Map/Tiles/Terrain")); }
        }

        public static ITexture2D MapCursorTexture
        {
            get { return GuiTextures.Find(texture => texture.MonoGameTexture.Name.Contains("Map/Cursor/Cursors")); }
        }

        public static ITexture2D MenuCursorTexture
        {
            get
            {
                return GuiTextures.Find(texture => texture.MonoGameTexture.Name.Contains("HUD/Cursor/MenuCursorArrow"));
            }
        }

        public static ITexture2D WindowTexture
        {
            get { return WindowTextures.Find(texture => texture.MonoGameTexture.Name.Contains("LightWindow")); }
        }

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

            AvailableMaps = new Dictionary<string, string>
            {
                {"Last Harbour", "Void_01.tmx"},
                {"Atheion Grassland", "Grass_01.tmx"},
                {"Romjack Mountain", "Snow_01.tmx"},
                {"Yaruuti Desert", "Desert_01.tmx"}
            };

            SpriteAtlas mainMenuLogoSpriteAtlas = new SpriteAtlas(MainMenuLogoTexture,
                new Vector2(MainMenuLogoTexture.Width, MainMenuLogoTexture.Height), 1);
            p1ControlMapper = new GameControlMapper(PlayerIndex.One);
            p2ControlMapper = new GameControlMapper(PlayerIndex.Two);

            GameContext.ActivePlayer = PlayerIndex.One;


            _gameContext = new GameContext(
                new MainMenuUI(mainMenuLogoSpriteAtlas),
                new MapSelectionMenuUI()
            );
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            TerrainTextures = ContentLoader.LoadTerrainSpriteTexture(Content);
            ActionTiles = ContentLoader.LoadActionTiles(Content);

            WhitePixel = ContentLoader.LoadWhitePixel(Content);
            WhiteGrid = ContentLoader.LoadWhiteGridOutline(Content);

            StatIcons = ContentLoader.LoadStatIcons(Content);

            UnitSprites = ContentLoader.LoadUnitSpriteTextures(Content);
            GuiTextures = ContentLoader.LoadCursorTextures(Content);
            WindowTextures = ContentLoader.LoadWindowTextures(Content);

            WindowFont = ContentLoader.LoadWindowFont(Content);
            MapFont = ContentLoader.LoadMapFont(Content);
            ResultsFont = ContentLoader.LoadResultsFont(Content);
            HeaderFont = ContentLoader.LoadHeaderFont(Content);
            MainMenuFont = ContentLoader.LoadMainMenuFont(Content);

            MainMenuLogoTexture = ContentLoader.LoadMainMenuBackground(Content);

            LargePortraitTextures = ContentLoader.LoadLargePortraits(Content);
            MediumPortraitTextures = ContentLoader.LoadMediumPortraits(Content);
            SmallPortraitTextures = ContentLoader.LoadSmallPortraits(Content);
            DiceTexture = ContentLoader.LoadDiceAtlas(Content);
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
            if (p1ControlMapper.Select())
            {
                NewGame(AvailableMaps.ElementAt(Random.Next(AvailableMaps.Count)).Value);
            }

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

            if (GameContext.CurrentGameState == GameContext.GameState.InGame)
            {
                _mapCamera.UpdateEveryFrame();
                _gameContext.UpdateCamera(_mapCamera);

                //Map Cursor Hover Logic
                MapSlice hoverTiles = MapContainer.GetMapSliceAtCursor();
                MapCursorHover.Hover(_gameContext.MapContext.CurrentTurnState, hoverTiles,
                    _gameContext.MapContext.MapUI);
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
                    //Render Map Select Screen
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, null);
                    _gameContext.MapSelectionMenuUI.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameContext.GameState.PauseScreen:
                    break;
                case GameContext.GameState.InGame:

                    //MAP LAYER
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, _mapCamera.CameraMatrix);
                    _gameContext.MapContext.MapContainer.Draw(spriteBatch);
                    spriteBatch.End();

                    //WINDOW LAYER
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, null);

                    if (_gameContext.MapContext.CurrentTurnState == MapContext.TurnState.UnitActing)
                    {
                        _gameContext.BattleContext.Draw(spriteBatch);
                    }
                    else
                    {
                        _gameContext.MapContext.MapUI.Draw(spriteBatch);
                    }

                    spriteBatch.End();
                    break;
                case GameContext.GameState.Results:

                    //Render Results
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, null);

                    _gameContext.ResultsUI.Draw(spriteBatch);

                    spriteBatch.End();
                    break;
                default:
                    base.Draw(gameTime);
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
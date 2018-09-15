using System;
using System.Collections.Generic;
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
        public static readonly Random Random = new Random();

        //Tile Size of Sprites
        public const int CellSize = 32;

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameControlMapper p1ControlMapper;
        private GameControlMapper p2ControlMapper;
        public static Vector2 ScreenSize { get; private set; }

        private static GameContext _gameContext;
        private static bool _quitting;

        public static ISpriteFont WindowFont { get; private set; }
        public static ISpriteFont MapFont { get; private set; }
        public static ISpriteFont ResultsFont { get; private set; }
        public static ISpriteFont HeaderFont { get; private set; }
        public static ISpriteFont MainMenuFont { get; private set; }

        public static List<ITexture2D> TerrainTextures { get; private set; }
        public static ITexture2D ActionTiles { get; private set; }
        public static ITexture2D WhitePixel { get; private set; }
        public static ITexture2D WhiteGrid { get; private set; }
        public static ITexture2D DiceTexture { get; private set; }
        public static ITexture2D StatIcons { get; private set; }
        public static ITexture2D MainMenuLogoTexture { get; private set; }

        public static List<ITexture2D> UnitSprites { get; private set; }
        public static List<ITexture2D> GuiTextures { get; private set; }
        public static List<ITexture2D> WindowTextures { get; private set; }

        public static List<ITexture2D> LargePortraitTextures { get; private set; }
        public static List<ITexture2D> MediumPortraitTextures { get; private set; }
        public static List<ITexture2D> SmallPortraitTextures { get; private set; }


        private MapCamera mapCamera;
        public static string[] MapFiles;

        private static PlayerIndex ActivePlayer { get; set; }

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
        public static void NewGame()
        {
            _gameContext.StartGame();
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

            mapCamera = new MapCamera(5, 0.05f);

            //TODO Map Path Hard-coded for now; remove me once map selector implemented
            MapFiles = new[]
            {
                "Grass_01.tmx",
                "Snow_01.tmx",
                "Desert_01.tmx"
            };

            ITexture2D windowTexture =
                WindowTextures.Find(texture => texture.MonoGameTexture.Name.Contains("LightWindow"));
            ITexture2D menuCursorTexture =
                GuiTextures.Find(texture => texture.MonoGameTexture.Name.Contains("HUD/Cursor/MenuCursorArrow"));
            SpriteAtlas mainMenuLogo = new SpriteAtlas(MainMenuLogoTexture,
                new Vector2(MainMenuLogoTexture.Width, MainMenuLogoTexture.Height), 1);
            p1ControlMapper = new GameControlMapper(PlayerIndex.One);
            p2ControlMapper = new GameControlMapper(PlayerIndex.Two);

            ActivePlayer = PlayerIndex.One;


            _gameContext = new GameContext(
                new MainMenuUI(menuCursorTexture, windowTexture, mainMenuLogo),
                windowTexture
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
                _gameContext.StartGame();
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

            if (p1ControlMapper.Y())
            {
                //TODO Remove this once debugging is no longer needed
                _gameContext.ResolveTurn();
            }

            //Set the controller based on the active team
            if (GameContext.CurrentGameState >= GameContext.GameState.InGame)
            {
                switch (GameContext.ActiveUnit.Team)
                {
                    case Team.Red:
                        ActivePlayer = PlayerIndex.One;
                        break;
                    case Team.Blue:
                        ActivePlayer = PlayerIndex.Two;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (ActivePlayer)
            {
                case PlayerIndex.One:
                    ControlContext.ListenForInputs(_gameContext, p1ControlMapper, mapCamera, MapContainer.MapCursor);
                    break;
                case PlayerIndex.Two:
                    ControlContext.ListenForInputs(_gameContext, p2ControlMapper, mapCamera, MapContainer.MapCursor);
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
                mapCamera.UpdateEveryFrame();
                _gameContext.UpdateCamera(mapCamera);

                //Map Cursor Hover Logic
                MapSlice hoverTiles = MapContainer.GetMapSliceAtCursor();
                MapCursorHover.Hover(_gameContext.MapContext.CurrentTurnState, hoverTiles, _gameContext.MapContext.MapUI);
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
                    //TODO Render Main VerticalMenu
                    //Render Results
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, null);
                    _gameContext.MainMenuUI.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameContext.GameState.InGame:

                    //MAP LAYER
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, mapCamera.CameraMatrix);
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
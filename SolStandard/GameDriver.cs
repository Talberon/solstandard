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
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Load;
using SolStandard.Utility.Monogame;
using TiledSharp;

namespace SolStandard
{
    enum GameLayer
    {
        Window,
        Map
    }

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
        private GameControlMapper controlMapper;
        public static Vector2 ScreenSize { get; private set; }

        private GameContext gameContext;

        public static List<ITexture2D> TerrainTextures { get; private set; }
        public static ITexture2D ActionTiles { get; private set; }
        public static ITexture2D WhitePixel { get; private set; }
        public static ITexture2D WhiteGrid { get; private set; }
        public static ISpriteFont WindowFont { get; private set; }
        public static ISpriteFont MapFont { get; private set; }
        public static ISpriteFont ResultsFont { get; private set; }
        public static ITexture2D DiceTexture { get; private set; }

        private static List<ITexture2D> UnitSprites { get; set; }
        private static List<ITexture2D> GuiTextures { get; set; }
        private static List<ITexture2D> WindowTextures { get; set; }
        private static List<ITexture2D> LargePortraitTextures { get; set; }
        private static List<ITexture2D> MediumPortraitTextures { get; set; }
        private static List<ITexture2D> SmallPortraitTextures { get; set; }

        private MapCamera mapCamera;

        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900
            };

            //HACK Move the window away from the top-left corner
            Window.Position = new Point(0, 50);

            Content.RootDirectory = "Content";
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

            ScreenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //TODO Map Path Hard-coded for now; remove me once map selector implemented
            const string mapPath = "Content/TmxMaps/NewFormat/Continent_02.tmx";
            TmxMap tmxMap = new TmxMap(mapPath);

            const string objectTypeDefaults = "Content/TmxMaps/objecttypes.xml";

            TmxMapParser mapParser = new TmxMapParser(
                tmxMap,
                TerrainTextures.Find(texture => texture.Name.Contains("Map/Tiles/WorldTileSet")),
                TerrainTextures.Find(texture => texture.Name.Contains("Map/Tiles/Terrain")),
                UnitSprites,
                objectTypeDefaults
            );
            
            controlMapper = new GameControlMapper();

            mapCamera = new MapCamera(5, 0.05f);

            ITexture2D cursorTexture = GuiTextures.Find(texture => texture.MonoGameTexture.Name.Contains("Cursors"));
            MapContainer gameMap = new MapContainer(mapParser.LoadMapGrid(), cursorTexture);

            List<GameUnit> unitsFromMap = UnitClassBuilder.GenerateUnitsFromMap(
                mapParser.LoadUnits(), LargePortraitTextures, MediumPortraitTextures,
                SmallPortraitTextures);

            Vector2 screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            ITexture2D windowTexture =
                WindowTextures.Find(texture => texture.MonoGameTexture.Name.Contains("LightWindow"));

            gameContext = new GameContext(
                new MapContext(gameMap, new MapUI(screenSize, windowTexture)),
                new BattleContext(new BattleUI(windowTexture)),
                new InitiativeContext(unitsFromMap, (Random.Next(2) == 0) ? Team.Blue : Team.Red),
                new ResultsUI(windowTexture)
            );

            gameContext.StartGame();
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

            UnitSprites = ContentLoader.LoadUnitSpriteTextures(Content);
            GuiTextures = ContentLoader.LoadCursorTextures(Content);
            WindowTextures = ContentLoader.LoadWindowTextures(Content);
            WindowFont = ContentLoader.LoadWindowFont(Content);
            MapFont = ContentLoader.LoadMapFont(Content);
            ResultsFont = ContentLoader.LoadResultsFont(Content);
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
            if (controlMapper.Select())
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                mapCamera.ZoomToCursor(4);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                mapCamera.ZoomToCursor(2);
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

            gameContext.EndTurnIfUnitIsDead();


            ControlContext.ListenForInputs(gameContext, controlMapper, mapCamera, MapContainer.MapCursor);

            mapCamera.UpdateEveryFrame();
            gameContext.UpdateCamera(mapCamera);

            //Map Cursor Hover Logic
            MapSlice hoverTiles = MapContainer.GetMapSliceAtCursor();
            MapCursorHover.Hover(gameContext.MapContext.CurrentTurnState, hoverTiles, gameContext.MapContext.MapUI);


            gameContext.MapContext.UpdateWindows();
            gameContext.ResultsUI.UpdateWindows();


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(50, 50, 50));

            //MAP LAYER
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, mapCamera.CameraMatrix);

            gameContext.MapContext.MapContainer.Draw(spriteBatch);


            spriteBatch.End();

            switch (GameContext.CurrentGameState)
            {
                case GameContext.GameState.MainMenu:
                    //TODO Render Main Menu
                    break;
                case GameContext.GameState.InGame:

                    //WINDOW LAYER
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, null);

                    if (gameContext.MapContext.CurrentTurnState == MapContext.TurnState.UnitActing)
                    {
                        gameContext.BattleContext.Draw(spriteBatch);
                    }
                    else
                    {
                        gameContext.MapContext.MapUI.Draw(spriteBatch);
                    }

                    spriteBatch.End();
                    break;
                case GameContext.GameState.Results:

                    //Render Results
                    spriteBatch.Begin(
                        SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                        null, SamplerState.PointClamp, null, null, null, null);

                    gameContext.ResultsUI.Draw(spriteBatch);

                    spriteBatch.End();
                    break;
                default:
                    base.Draw(gameTime);
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
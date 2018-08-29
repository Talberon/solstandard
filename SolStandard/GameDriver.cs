using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Rules;
using SolStandard.Rules.Controls;
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

        public static ITexture2D TerrainTextures { get; private set; }
        public static ITexture2D WhitePixel { get; private set; }
        public static ISpriteFont WindowFont { get; private set; }
        public static ISpriteFont MapFont { get; private set; }
        public static ITexture2D DiceTexture { get; private set; }

        private static List<ITexture2D> UnitSprites { get; set; }
        private static List<ITexture2D> GuiTextures { get; set; }
        private static List<ITexture2D> WindowTextures { get; set; }
        private static List<ITexture2D> LargePortraitTextures { get; set; }
        private static List<ITexture2D> MediumPortraitTextures { get; set; }
        private static List<ITexture2D> SmallPortraitTextures { get; set; }


        private MapCamera mapCamera;

        private MapHudGenerator mapHudGenerator;

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

            const string
                mapPath =
                    "Content/TmxMaps/Arena_3.tmx"; //TODO Hard-coded for now; remove me once map selector implemented
            const string objectTypeDefaults = "Content/TmxMaps/objecttypes.xml";
            TmxMap tmxMap = new TmxMap(mapPath);
            TmxMapParser mapParser = new TmxMapParser(tmxMap, TerrainTextures, UnitSprites, objectTypeDefaults);
            controlMapper = new GameControlMapper();

            mapCamera = new MapCamera(10);
            mapCamera.SetCameraZoom(1.8f);

            ITexture2D cursorTexture = GuiTextures.Find(texture => texture.MonoGameTexture.Name.Contains("Cursor"));
            MapContainer gameMap = new MapContainer(mapParser.LoadMapGrid(), cursorTexture);

            List<GameUnit> unitsFromMap = UnitClassBuilder.GenerateUnitsFromMap(
                mapParser.LoadUnits(), LargePortraitTextures, MediumPortraitTextures,
                SmallPortraitTextures);

            Vector2 screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            gameContext = new GameContext(new MapContext(gameMap),
                new BattleContext(new BattleUI(screenSize), WindowTextures.First()), new MapUI(screenSize),
                unitsFromMap);

            ITexture2D windowTexture =
                WindowTextures.Find(texture => texture.MonoGameTexture.Name.Contains("LightWindow"));
            mapHudGenerator = new MapHudGenerator(windowTexture);
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
            WhitePixel = ContentLoader.LoadWhitePixel(Content);

            UnitSprites = ContentLoader.LoadUnitSpriteTextures(Content);
            GuiTextures = ContentLoader.LoadCursorTextures(Content);
            WindowTextures = ContentLoader.LoadWindowTextures(Content);
            WindowFont = ContentLoader.LoadWindowFont(Content);
            MapFont = ContentLoader.LoadMapFont(Content);
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

            //TODO Introduce enum to represent game state before choosing which Control set to listen for
            MapSceneControls.ListenForInputs(gameContext.MapContext, gameContext.BattleContext, controlMapper,
                mapCamera, gameContext.MapContext.MapContainer.MapCursor, gameContext.MapUI);


            Vector2 mapSize = gameContext.MapContext.MapContainer.MapGridSize;
            mapCamera.CorrectCameraToCursor(gameContext.MapContext.MapContainer.MapCursor, mapSize);
            mapCamera.PanCameraToTarget();


            //Map Cursor Hover Logic
            MapSlice hoverTiles = gameContext.MapContext.MapContainer.GetMapSliceAtCursor();
            MapCursorHover.Hover(gameContext.MapContext.CurrentTurnState, gameContext.MapUI, hoverTiles,
                mapHudGenerator);


            //Initiative Window
            gameContext.MapUI.InitiativeWindow =
                mapHudGenerator.GenerateInitiativeWindow(GameContext.Units);

            //Turn Window
            Vector2 turnWindowSize = new Vector2(265, gameContext.MapUI.InitiativeWindow.Height);
            gameContext.MapUI.TurnWindow = mapHudGenerator.GenerateTurnWindow(turnWindowSize);


            //Help Window TODO make this context-sensitive
            string helpText = "HELP: Lorem ipsum dolor sit amet conseceteur novus halonus."
                              + "\nAdditional information will appear here to help you play the game.";

            gameContext.MapUI.HelpTextWindow = mapHudGenerator.GenerateHelpWindow(helpText);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, mapCamera.CameraMatrix);
            gameContext.MapContext.MapContainer.Draw(spriteBatch);

            base.Draw(gameTime);
            spriteBatch.End();

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
                gameContext.MapUI.Draw(spriteBatch);
            }


            spriteBatch.End();
        }
    }
}
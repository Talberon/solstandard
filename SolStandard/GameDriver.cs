using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Map.Objects;
using SolStandard.Map.Objects.Cursor;
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
        //Tile Size of Sprites
        public const int CellSize = 32;

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameControlMapper controlMapper;

        private GameContainer container;

        private ITexture2D terrainTextures;
        private List<ITexture2D> unitSprites;
        private List<ITexture2D> guiTextures;
        private List<ITexture2D> windowTextures;
        private List<ITexture2D> largePortraitTextures;
        private List<ITexture2D> mediumPortraitTextures;
        private List<ITexture2D> smallPortraitTextures;
        private ISpriteFont windowFont;
        private MapCamera mapCamera;

        private MapStaticHud mapStaticHud;

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

            const string
                mapPath =
                    "Content/TmxMaps/Arena_3.tmx"; //TODO Hard-coded for now; remove me once map selector implemented
            const string objectTypeDefaults = "Content/TmxMaps/objecttypes.xml";
            TmxMap tmxMap = new TmxMap(mapPath);
            TmxMapParser mapParser = new TmxMapParser(tmxMap, terrainTextures, unitSprites, objectTypeDefaults);
            controlMapper = new GameControlMapper();

            mapCamera = new MapCamera(10);
            mapCamera.SetCameraZoom(1.8f);

            ITexture2D cursorTexture = guiTextures.Find(texture => texture.MonoGameTexture.Name.Contains("Cursor"));
            MapLayer gameMap = new MapLayer(mapParser.LoadMapGrid(), cursorTexture);

            List<GameUnit> unitsFromMap = UnitClassBuilder.GenerateUnitsFromMap(
                (MapEntity[,]) gameMap.GameGrid[(int) Layer.Units], largePortraitTextures, mediumPortraitTextures,
                smallPortraitTextures);


            container = new GameContainer(gameMap,
                new MapUI(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)),
                unitsFromMap);

            ITexture2D windowTexture =
                windowTextures.Find(texture => texture.MonoGameTexture.Name.Contains("LightWindow"));
            mapStaticHud = new MapStaticHud(windowFont, windowTexture);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            terrainTextures = ContentLoader.LoadTerrainSpriteTexture(Content);
            unitSprites = ContentLoader.LoadUnitSpriteTextures(Content);
            guiTextures = ContentLoader.LoadCursorTextures(Content);
            windowTextures = ContentLoader.LoadWindowTextures(Content);
            windowFont = ContentLoader.LoadWindowFont(Content);
            largePortraitTextures = ContentLoader.LoadLargePortraits(Content);
            mediumPortraitTextures = ContentLoader.LoadMediumPortraits(Content);
            smallPortraitTextures = ContentLoader.LoadSmallPortraits(Content);
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
            MapSceneControls.ListenForInputs(controlMapper, mapCamera, container.MapLayer.MapCursor,
                container.MapUI, container.MapLayer.GetMapSliceAtCursor(), container.Units);


            Vector2 screenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Vector2 mapSize = container.MapLayer.MapGridSize;
            mapCamera.CorrectCameraToCursor(container.MapLayer.MapCursor, screenSize, mapSize);
            mapCamera.PanCameraToTarget();


            //Map Cursor Hover Logic
            MapSlice hoverTiles = container.MapLayer.GetMapSliceAtCursor();
            MapCursorHover.Hover(container.MapUI, hoverTiles, container.Units, mapStaticHud);

            //Initiative Window
            container.MapUI.InitiativeWindow =
                mapStaticHud.GenerateInitiativeWindow(container.Units);

            //Turn Window
            Vector2 turnWindowSize = new Vector2(265, container.MapUI.InitiativeWindow.Height);
            container.MapUI.TurnWindow = mapStaticHud.GenerateTurnWindow(turnWindowSize);


            //Help Window TODO make this context-sensitive
            string helpText = "HELP: Lorem ipsum dolor sit amet conseceteur novus halonus."
                              + "\nAdditional information will appear here to help you play the game.";

            container.MapUI.HelpTextWindow = mapStaticHud.GenerateHelpWindow(helpText);


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
            container.MapLayer.Draw(spriteBatch);
            base.Draw(gameTime);
            spriteBatch.End();

            //WINDOW LAYER
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);
            container.MapUI.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
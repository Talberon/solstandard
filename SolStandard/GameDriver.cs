using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Containers;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Logic;
using SolStandard.Map;
using SolStandard.Map.Camera;
using SolStandard.Map.Objects;
using SolStandard.Map.Objects.Cursor;
using SolStandard.Utility;
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

            ITexture2D cursorTexture = guiTextures.Find(texture => texture.GetTexture2D().Name.Contains("Cursor"));
            MapLayer gameMap = new MapLayer(mapParser.LoadMapGrid(), cursorTexture);

            List<GameUnit> unitsFromMap = UnitClassBuilder.GenerateUnitsFromMap(
                (MapEntity[,]) gameMap.GetGameGrid()[(int) Layer.Units],
                largePortraitTextures, mediumPortraitTextures, smallPortraitTextures);


            container = new GameContainer(gameMap,
                new WindowLayer(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)),
                unitsFromMap);
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

            if (controlMapper.Start())
            {
                mapCamera.SetTargetCameraPosition(new Vector2(0));
            }

            if (controlMapper.Down())
            {
                container.GetMapLayer().GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Down));
            }

            if (controlMapper.Left())
            {
                container.GetMapLayer().GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Left));
            }

            if (controlMapper.Right())
            {
                container.GetMapLayer().GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Right));
            }

            if (controlMapper.Up())
            {
                container.GetMapLayer().GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Up));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Down);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Left);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Right);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Up);
            }

            Vector2 screenSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Vector2 mapSize = container.GetMapLayer().MapSize();
            mapCamera.CorrectCameraToCursor(container.GetMapLayer().GetMapCursor(), screenSize, mapSize);
            mapCamera.PanCameraToTarget();


            {
                //TODO move to its own method :: Selected Unit Windows
                ITexture2D windowTexture =
                    windowTextures.Find(texture => texture.GetTexture2D().Name.Contains("LightWindow"));

                GameUnit selectedUnit = UnitSelector.SelectUnit(container.GetUnits(),
                    (MapEntity[,]) container.GetMapLayer().GetGameGrid()[(int) Layer.Units],
                    container.GetMapLayer().GetMapCursor().GetMapCoordinates());

                if (selectedUnit != null)
                {
                    //Create Unit Window Content
                    IRenderable selectedUnitPortrait = new WindowContent(new TileCell(selectedUnit.LargePortrait,
                        selectedUnit.LargePortrait.GetHeight(), 1));

                    IRenderable selectedUnitInfo =
                        new RenderText(windowFont, selectedUnit.Id + ":\n" + selectedUnit.Stats);


                    string windowLabel = "Selected Unit: " + selectedUnit.Id;


                    bool windowExists = false;
                    foreach (Window window in container.GetWindowLayer().ExtraWindows)
                    {
                        if (window.WindowLabel.Equals(windowLabel))
                        {
                            windowExists = true;
                        }
                    }

                    if (!windowExists)
                    {
                        Color windowColour = Color.Gray;

                        if (selectedUnit.UnitTeam == Team.Blue)
                        {
                            windowColour = new Color(75, 75, 150, 200);
                        }
                        else if (selectedUnit.UnitTeam == Team.Red)
                        {
                            windowColour = new Color(150, 75, 75, 200);
                        }

                        container.GetWindowLayer().LeftUnitPortraitWindow = new Window(windowLabel, windowTexture,
                            selectedUnitPortrait, windowColour);
                        container.GetWindowLayer().RightUnitPortraitWindow = new Window(windowLabel, windowTexture,
                            selectedUnitPortrait, windowColour);

                        container.GetWindowLayer().LeftUnitDetailWindow = new Window(windowLabel, windowTexture,
                            selectedUnitInfo, windowColour);
                        container.GetWindowLayer().RightUnitDetailWindow = new Window(windowLabel, windowTexture,
                            selectedUnitInfo, windowColour);
                    }
                }
                else
                {
                    container.GetWindowLayer().LeftUnitPortraitWindow = null;
                    container.GetWindowLayer().RightUnitPortraitWindow = null;
                }

                container.GetWindowLayer().DebugWindow = new Window("Debug", windowTexture,
                    new RenderText(windowFont, string.Join(",", container.GetWindowLayer().ExtraWindows)),
                    new Color(0, 100, 0, 200));


                {
                    //TODO move to its own method :: Initiative Window
                    const int maxInitiativeSize = 10;
                    int initiativeListLength = (container.GetUnits().Count > maxInitiativeSize)
                        ? maxInitiativeSize
                        : container.GetUnits().Count;

                    IRenderable[,] unitListGrid = new IRenderable[1, initiativeListLength];


                    for (int i = 0; i < unitListGrid.GetLength(1); i++)
                    {
                        IRenderable unitPortraitWindow = new WindowContent(
                            new TileCell(
                                container.GetUnits()[i].MediumPortrait,
                                container.GetUnits()[i].MediumPortrait.GetHeight(), 1
                            )
                        );
                        unitListGrid[0, i] = unitPortraitWindow;
                    }

                    WindowContentGrid unitListContentGrid = new WindowContentGrid(unitListGrid, 3);

                    container.GetWindowLayer().InitiativeWindow =
                        new Window("Initiative", windowTexture, unitListContentGrid, new Color(100, 100, 100, 225));
                }

                {
                    //TODO move to its own method :: Turn Window
                    WindowContentGrid unitListContentGrid = new WindowContentGrid(
                        new IRenderable[,]
                        {
                            {
                                new RenderText(windowFont,
                                    "EXAMPLE//Current Turn: 0") //TODO make dynamic; not hard-coded
                            },
                            {
                                new RenderText(windowFont,
                                    "EXAMPLE//Active Team: Blue") //TODO make dynamic; not hard-coded
                            },
                            {
                                new RenderText(windowFont,
                                    "EXAMPLE//Active Unit: Knight") //TODO make dynamic; not hard-coded
                            }
                        },
                        1);

                    container.GetWindowLayer().TurnWindow =
                        new Window("Turn Counter", windowTexture, unitListContentGrid, new Color(100, 100, 100, 225),
                            new Vector2(265, container.GetWindowLayer().InitiativeWindow.GetHeight()));
                }

                {
                    //TODO move to its own method :: Terrain Window
                    Vector2 cursorCoordinates = container.GetMapLayer().GetMapCursor().GetMapCoordinates();
                    MapEntity selectedTerrain =
                        (MapEntity) container.GetMapLayer().GetGameGrid()[(int) Layer.Entities][
                            (int) cursorCoordinates.X, (int) cursorCoordinates.Y];

                    WindowContentGrid terrainContentGrid;

                    if (selectedTerrain != null)
                    {
                        IRenderable terrainSprite = selectedTerrain.GetSprite();

                        string terrainInfo = "Terrain: " + selectedTerrain.Name
                                                         + "\n"
                                                         + "Type: " + selectedTerrain.Type
                                                         + "\n"
                                                         + "Properties:\n" + string.Join("\n",
                                                             selectedTerrain.TiledProperties);

                        terrainContentGrid = new WindowContentGrid(
                            new[,]
                            {
                                {
                                    terrainSprite,
                                    new RenderText(windowFont, terrainInfo)
                                }
                            },
                            1);
                    }
                    else
                    {
                        terrainContentGrid = new WindowContentGrid(
                            new IRenderable[,]
                            {
                                {
                                    new RenderText(windowFont, "None ")
                                }
                            },
                            1);
                    }

                    container.GetWindowLayer().TerrainWindow =
                        new Window("Terrain Info", windowTexture, terrainContentGrid, new Color(100, 150, 100, 220));
                }

                {
                    //TODO move to its own method :: Help Text Window

                    IRenderable helpText = new RenderText(windowFont,
                        "HELP: Lorem ipsum dolor sit amet conseceteur novus halonus."
                        + "\nAdditional information will appear here to help you play the game.");
                    container.GetWindowLayer().HelpTextWindow =
                        new Window("Help Text", windowTexture, helpText, new Color(30, 30, 30, 150));
                }
            }


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
                null, SamplerState.PointClamp, null, null, null, mapCamera.GetCameraMatrix());
            container.GetMapLayer().Draw(spriteBatch);
            base.Draw(gameTime);
            spriteBatch.End();

            //WINDOW LAYER
            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);
            container.GetWindowLayer().Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
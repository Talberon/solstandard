using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Map;
using SolStandard.Map.Objects;
using SolStandard.Utility.Load;
using SolStandard.Utility.Monogame;
using System.Collections.Generic;
using SolStandard.Map.Objects.Cursor;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Camera;
using TiledSharp;

namespace SolStandard
{
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

        private MapContainer gameMap;
        private ITexture2D terrainTextures;
        private List<ITexture2D> unitSprites;
        private List<ITexture2D> guiTextures;
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

            const string mapPath = "Content/TmxMaps/Arena_2.tmx"; //TODO Hard-coded for now; remove me once map selector implemented
            TmxMap tmxMap = new TmxMap(mapPath);
            TmxMapParser mapParser = new TmxMapParser(tmxMap, terrainTextures, unitSprites);
            controlMapper = new GameControlMapper();

            mapCamera = new MapCamera(10);
            mapCamera.SetCameraZoom(1.8f);

            ITexture2D cursorTexture = guiTextures.Find(texture => texture.GetTexture2D().Name.Contains("Cursor"));

            gameMap = new MapContainer(mapParser.LoadMapGrid(), cursorTexture);
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
            guiTextures = ContentLoader.LoadGuiTextures(Content);
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
                gameMap.GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Down));
            }

            if (controlMapper.Left())
            {
                gameMap.GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Left));
            }

            if (controlMapper.Right())
            {
                gameMap.GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Right));
            }

            if (controlMapper.Up())
            {
                gameMap.GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Up));
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
            Vector2 mapSize = gameMap.MapSize();

            mapCamera.CorrectCameraToCursor(gameMap.GetMapCursor(), screenSize, mapSize);
            mapCamera.PanCameraToTarget();

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

            //Draw tiles in Map Grid
            foreach (MapObject[,] layer in gameMap.GetGameGrid())
            {
                foreach (MapObject tile in layer)
                {
                    if (tile != null)
                        tile.Draw(spriteBatch);
                }
            }

            //Draw map cursor
            gameMap.GetMapCursor().Draw(spriteBatch);

            base.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
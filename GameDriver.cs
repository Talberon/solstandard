using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Map;
using SolStandard.Map.Objects;
using SolStandard.Utility.Load;
using SolStandard.Utility.Monogame;
using System.Collections.Generic;
using SolStandard.Map.Objects.Cursor;
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
        public const int CELL_SIZE = 32;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;


        private MapContainer gameMap;
        private ITexture2D terrainTextures;
        private List<ITexture2D> unitSprites;
        private List<ITexture2D> guiTextures;
        private MapCamera mapCamera;

        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            
            //Move the window away from the top-left corner
            this.Window.Position = new Point(0, 50);
            
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
            // TODO: Add your initialization logic here
            string mapPath = "Content/TmxMaps/Arena_2.tmx";
            TmxMap tmxMap = new TmxMap(mapPath);
            TmxMapParser mapParser = new TmxMapParser(tmxMap, terrainTextures, unitSprites);
            
            mapCamera = new MapCamera(10);
            //FIXME remove me
            mapCamera.SetCameraZoom(1.4f);

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

            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            
            //TODO Temporary; remove this after implementing proper controls
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                mapCamera.SetTargetCameraPosition(new Vector2(0));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Down);
                gameMap.GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Down));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Left);
                gameMap.GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Left));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Right);
                gameMap.GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Right));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                mapCamera.MoveCameraInDirection(CameraDirection.Up);
                gameMap.GetMapCursor().MoveCursorInDirection((MapCursor.CursorDirection.Up));
            }

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

            // TODO: Add your drawing code here

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
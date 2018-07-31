using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolStandard.Map;
using SolStandard.Map.Objects;
using SolStandard.Utility.Load;
using SolStandard.Utility.Monogame;
using System.Collections.Generic;
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

        public GameDriver()
        {
            graphics = new GraphicsDeviceManager(this);
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

            
            gameMap = mapParser.LoadMap();
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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin(
                SpriteSortMode.Deferred, //Use deferred instead of texture to render in order of .Draw() calls
                null, SamplerState.PointClamp, null, null, null, null);


            foreach (MapObject[,] layer in gameMap.GetGameGrid())
            {
                foreach (MapObject tile in layer)
                {
                    if (tile != null)
                        tile.Draw(spriteBatch);
                }
            }

            base.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
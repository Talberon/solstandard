using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Camera;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Map
{
    public class MapContainer
    {
        public const int MapToastIconSize = 16;
        private static List<MapElement[,]> _gameGrid;
        public MapCursor MapCursor { get; }
        public IMapCamera MapCamera { get; }
        private static ToastWindow ToastWindow { get; set; }
        public List<CreepEntity> MapSummons { get; }
        public List<IItem> MapLoot { get; }

        public MapContainer(List<MapElement[,]> gameGrid, ITexture2D cursorTexture, List<CreepEntity> mapSummons,
            List<IItem> mapLoot)
        {
            MapSummons = mapSummons;
            _gameGrid = gameGrid;
            MapCursor = BuildMapCursor(cursorTexture);
            // MapCamera = new MapCamera(5, 0.05f);
            MapCamera = new NeoMapCamera(
                new OrthographicCamera(GameDriver.BoxingViewportAdapter),
                new CameraSmoother(0.5f, 2f)
            );
            MapLoot = mapLoot;
        }

        public MapContainer(List<MapElement[,]> gameGrid, ITexture2D cursorTexture)
            : this(gameGrid, cursorTexture, new List<CreepEntity>(), new List<IItem>())
        {
            //Used by MapSelect
        }

        public IItem GetRandomItemFromPool(string poolName)
        {
            List<IItem> poolItems = GetPoolItems(poolName);

            return poolItems.Count <= 0 ? null : poolItems[GameDriver.Random.Next(poolItems.Count)];
        }

        public List<IItem> GetPoolItems(string poolName)
        {
            List<IItem> poolItems = MapLoot
                .Where(item => item.ItemPool != string.Empty)
                .ToList()
                .FindAll(item => item.ItemPool == poolName);
            return poolItems;
        }

        private static MapCursor BuildMapCursor(ITexture2D cursorTexture)
        {
            var cursorSprite = new SpriteAtlas(cursorTexture, GameDriver.CellSizeVector);
            var cursorStartPosition = new Vector2(0);
            return new MapCursor(cursorSprite, cursorStartPosition, MapGridSize);
        }


        public static ReadOnlyCollection<MapElement[,]> GameGrid => _gameGrid.AsReadOnly();

        public static Vector2 MapGridSize => new Vector2(_gameGrid[0].GetLength(0), _gameGrid[0].GetLength(1));

        public static Vector2 MapScreenSizeInPixels =>
            new Vector2(_gameGrid[0].GetLength(0), _gameGrid[0].GetLength(1))
            * GameDriver.CellSize * GlobalContext.MapCamera.CurrentZoom;

        private static void AddNewToastAtMapPixelCoordinates(IRenderable content, Vector2 mapPixelCoordinates,
            int lifetimeInFrames)
        {
            ToastWindow = new ToastWindow(content, mapPixelCoordinates, lifetimeInFrames);
        }

        public void AddNewToastAtMapCellCoordinates(IRenderable content, Vector2 mapCoordinates, int lifetimeInFrames)
        {
            //Place the toast to the right of the designated map coordinates
            AddNewToastAtMapPixelCoordinates(
                content,
                (mapCoordinates + new Vector2(1, 0)) * GameDriver.CellSize,
                lifetimeInFrames
            );
        }

        public void AddNewToastAtMapCursor(IRenderable content, int lifetimeInFrames)
        {
            //Set the toast to the right of the cursor
            AddNewToastAtMapCellCoordinates(content, MapCursor.MapCoordinates, lifetimeInFrames);
        }

        public void AddNewToastAtUnit(UnitEntity unitEntity, IRenderable content, int lifetimeInFrames)
        {
            if (unitEntity == null)
            {
                //Place the toast at the cursor if the unit is dead
                AddNewToastAtMapCursor(content, lifetimeInFrames);
            }
            else
            {
                //Set the toast to the right of the unit
                AddNewToastAtMapCellCoordinates(content, unitEntity.MapCoordinates, lifetimeInFrames);
            }
        }

        public void AddNewToastAtMapCellCoordinates(string toastMessage, Vector2 mapCoordinates, int lifetimeInFrames)
        {
            IRenderable toastContent = new RenderText(AssetManager.MapFont, toastMessage);
            AddNewToastAtMapCellCoordinates(toastContent, mapCoordinates, lifetimeInFrames);
        }

        public void AddNewToastAtMapCursor(string toastMessage, int lifetimeInFrames)
        {
            //Set the toast to the right of the cursor
            AddNewToastAtMapCellCoordinates(toastMessage, MapCursor.MapCoordinates, lifetimeInFrames);
        }

        public void AddNewToastAtUnit(UnitEntity unitEntity, string toastMessage, int lifetimeInFrames)
        {
            AddNewToastAtUnit(unitEntity, new RenderText(AssetManager.MapFont, toastMessage), lifetimeInFrames);
        }

        public static void ClearToasts()
        {
            ToastWindow = null;
        }

        public static void ClearDynamicAndPreviewGrids()
        {
            ClearDynamicGrid();
            ClearPreviewGrid();
        }

        private static void ClearDynamicGrid()
        {
            _gameGrid[(int) Layer.Dynamic] = new MapElement[_gameGrid[(int) Layer.Dynamic].GetLength(0),
                _gameGrid[(int) Layer.Dynamic].GetLength(1)];
        }

        public static void ClearPreviewGrid()
        {
            _gameGrid[(int) Layer.Preview] = new MapElement[_gameGrid[(int) Layer.Preview].GetLength(0),
                _gameGrid[(int) Layer.Preview].GetLength(1)];
        }

        public MapSlice GetMapSliceAtCursor()
        {
            return GetMapSliceAtCoordinates(MapCursor.MapCoordinates);
        }

        public static IEnumerable<MapEntity> GetMapEntities()
        {
            var entities = new List<MapEntity>();

            int mapHeight = (int) MapGridSize.Y;
            int mapWidth = (int) MapGridSize.X;

            for (int row = 0; row < mapHeight; row++)
            {
                for (int column = 0; column < mapWidth; column++)
                {
                    if (GameGrid[(int) Layer.Entities][column, row] is MapEntity currentEntity)
                        entities.Add(currentEntity);

                    if (GameGrid[(int) Layer.Items][column, row] is MapEntity currentItem) entities.Add(currentItem);
                }
            }

            return entities;
        }

        public static List<MapElement> GetMapElementsFromLayer(Layer layer)
        {
            var elements = new List<MapElement>();

            int mapHeight = (int) MapGridSize.Y;
            int mapWidth = (int) MapGridSize.X;

            for (int row = 0; row < mapHeight; row++)
            {
                for (int column = 0; column < mapWidth; column++)
                {
                    MapElement currentTile = GameGrid[(int) layer][column, row];

                    if (currentTile != null) elements.Add(currentTile);
                }
            }

            return elements;
        }

        public static MapSlice GetMapSliceAtCoordinates(Vector2 coordinates)
        {
            if (WorldContext.CoordinatesWithinMapBounds(coordinates))
            {
                int column = (int) coordinates.X;
                int row = (int) coordinates.Y;

                UnitEntity unit = UnitSelector.FindOtherUnitEntityAtCoordinates(coordinates, null);
                MapElement dynamic = _gameGrid[(int) Layer.Dynamic][column, row];
                MapElement preview = _gameGrid[(int) Layer.Preview][column, row];
                var entity = (TerrainEntity) _gameGrid[(int) Layer.Entities][column, row];
                var item = (TerrainEntity) _gameGrid[(int) Layer.Items][column, row];
                var collide = (MapTile) _gameGrid[(int) Layer.Collide][column, row];
                var terrainDecoration = (MapTile) _gameGrid[(int) Layer.TerrainDecoration][column, row];
                var terrain = (MapTile) _gameGrid[(int) Layer.Terrain][column, row];

                return new MapSlice(coordinates, unit, preview, dynamic, entity, item, collide, terrainDecoration,
                    terrain);
            }

            return new MapSlice(Vector2.Zero, null, null, null, null, null, null, null, null);
        }

        private static void DrawMapGrid(SpriteBatch spriteBatch)
        {
            for (int col = 0; col < _gameGrid[0].GetLength(0); col++)
            {
                for (int row = 0; row < _gameGrid[0].GetLength(1); row++)
                {
                    spriteBatch.Draw(AssetManager.WhiteGrid.MonoGameTexture,
                        new Vector2(col, row) * GameDriver.CellSize,
                        new Color(0, 0, 0, 20));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MapElement tile in _gameGrid[(int) Layer.Terrain])
            {
                tile?.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.TerrainDecoration])
            {
                tile?.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.Collide])
            {
                tile?.Draw(spriteBatch);
            }

            DrawMapGrid(spriteBatch);

            foreach (MapElement tile in _gameGrid[(int) Layer.Entities])
            {
                tile?.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.Items])
            {
                tile?.Draw(spriteBatch);
            }

            GlobalContext.Units.ForEach(unit => unit.DrawAuras(spriteBatch));

            foreach (MapElement tile in _gameGrid[(int) Layer.Dynamic])
            {
                tile?.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.Preview])
            {
                tile?.Draw(spriteBatch);
            }

            GlobalContext.Units.ForEach(unit => unit.UnitEntity?.Draw(spriteBatch));

            foreach (MapElement tile in _gameGrid[(int) Layer.Overlay])
            {
                tile?.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.OverlayEffect])
            {
                tile?.Draw(spriteBatch);
            }

            if (ToastWindow != null)
            {
                ToastWindow.Draw(spriteBatch);

                if (ToastWindow.Expired) ToastWindow = null;
            }

            MapCursor.Draw(spriteBatch);
        }
    }
}
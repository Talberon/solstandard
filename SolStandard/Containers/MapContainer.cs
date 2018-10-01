using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers
{
    public class MapContainer
    {
        private static List<MapElement[,]> _gameGrid;
        public static MapCursor MapCursor { get; private set; }

        public MapContainer(List<MapElement[,]> gameGrid, ITexture2D cursorTexture)
        {
            _gameGrid = gameGrid;
            MapCursor = BuildMapCursor(cursorTexture);
        }

        private static MapCursor BuildMapCursor(ITexture2D cursorTexture)
        {
            SpriteAtlas cursorSprite = new SpriteAtlas(cursorTexture, new Vector2(GameDriver.CellSize), 1);
            Vector2 cursorStartPosition = new Vector2(0);
            return new MapCursor(cursorSprite, cursorStartPosition, MapGridSize);
        }


        public static ReadOnlyCollection<MapElement[,]> GameGrid
        {
            get { return _gameGrid.AsReadOnly(); }
        }

        public static Vector2 MapGridSize
        {
            get { return new Vector2(_gameGrid[0].GetLength(0), _gameGrid[0].GetLength(1)); }
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

        public static MapSlice GetMapSliceAtCursor()
        {
            return GetMapSliceAtCoordinates(MapCursor.MapCoordinates);
        }

        public static MapSlice GetMapSliceAtCoordinates(Vector2 coordinates)
        {
            if (MapContext.CoordinatesWithinMapBounds(coordinates))
            {
                int column = (int) coordinates.X;
                int row = (int) coordinates.Y;

                UnitEntity unit = UnitSelector.FindOtherUnitEntityAtCoordinates(coordinates, null);
                MapElement dynamic = _gameGrid[(int) Layer.Dynamic][column, row];
                MapElement preview = _gameGrid[(int) Layer.Preview][column, row];
                TerrainEntity entity = (TerrainEntity) _gameGrid[(int) Layer.Entities][column, row];
                MapTile collide = (MapTile) _gameGrid[(int) Layer.Collide][column, row];
                MapTile terrainDecoration = (MapTile) _gameGrid[(int) Layer.TerrainDecoration][column, row];
                MapTile terrain = (MapTile) _gameGrid[(int) Layer.Terrain][column, row];

                return new MapSlice(coordinates, unit, preview, dynamic, entity, collide, terrainDecoration, terrain);
            }

            return new MapSlice(Vector2.Zero, null, null, null, null, null, null, null);
        }

        private void DrawMapGrid(SpriteBatch spriteBatch)
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
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.TerrainDecoration])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.Collide])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            DrawMapGrid(spriteBatch);

            foreach (MapElement tile in _gameGrid[(int) Layer.Entities])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.Preview])
            {
                if (tile != null)
                    tile.Draw(spriteBatch, new Color(255, 255, 255, 200));
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.Dynamic])
            {
                if (tile != null)
                    tile.Draw(spriteBatch, new Color(255, 255, 255, 180));
            }

            foreach (GameUnit unit in GameContext.Units)
            {
                if (unit.UnitEntity != null)
                {
                    unit.UnitEntity.Draw(spriteBatch);
                }
            }

            MapCursor.Draw(spriteBatch);
        }
    }
}
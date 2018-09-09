using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
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

        public static void ClearDynamicGrid()
        {
            _gameGrid[(int) Layer.Dynamic] = new MapElement[_gameGrid[(int) Layer.Dynamic].GetLength(0),
                _gameGrid[(int) Layer.Dynamic].GetLength(1)];
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

                MapEntity unit = UnitSelector.FindOtherUnitEntityAtCoordinates(coordinates, null);
                MapElement dynamic = _gameGrid[(int) Layer.Dynamic][column, row];
                MapEntity entity = (MapEntity) _gameGrid[(int) Layer.Entities][column, row];
                MapTile collide = (MapTile) _gameGrid[(int) Layer.Collide][column, row];
                MapTile terrain = (MapTile) _gameGrid[(int) Layer.Terrain][column, row];

                return new MapSlice(unit, dynamic, entity, collide, terrain);
            }

            return new MapSlice(null, null, null, null, null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MapElement tile in _gameGrid[(int) Layer.Terrain])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.Collide])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            foreach (MapElement tile in _gameGrid[(int) Layer.Entities])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
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
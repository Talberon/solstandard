using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Logic;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers
{
    public class MapContainer
    {
        private readonly List<MapElement[,]> gameGrid;
        private readonly MapCursor mapCursor;

        public MapContainer(List<MapElement[,]> gameGrid, ITexture2D cursorTexture)
        {
            this.gameGrid = gameGrid;
            mapCursor = BuildMapCursor(cursorTexture);
        }

        private MapCursor BuildMapCursor(ITexture2D cursorTexture)
        {
            SpriteAtlas cursorCell = new SpriteAtlas(cursorTexture, GameDriver.CellSize, 1);
            Vector2 cursorStartPosition = new Vector2(0);
            return new MapCursor(cursorCell, cursorStartPosition, MapGridSize);
        }

        public MapCursor MapCursor
        {
            get { return mapCursor; }
        }

        public ReadOnlyCollection<MapElement[,]> GameGrid
        {
            get { return gameGrid.AsReadOnly(); }
        }

        public Vector2 MapGridSize
        {
            get { return new Vector2(gameGrid[0].GetLength(0), gameGrid[0].GetLength(1)); }
        }

        public void ClearDynamicGrid()
        {
            gameGrid[(int) Layer.Dynamic] = new MapElement[gameGrid[(int) Layer.Dynamic].GetLength(0),
                gameGrid[(int) Layer.Dynamic].GetLength(1)];
        }

        public MapSlice GetMapSliceAtCursor()
        {
            return GetMapSliceAtCoordinates(mapCursor.MapCoordinates);
        }

        public MapSlice GetMapSliceAtCoordinates(Vector2 coordinates)
        {
            int column = (int) coordinates.X;
            int row = (int) coordinates.Y;

            MapEntity unit = UnitSelector.FindOtherUnitEntityAtCoordinates(coordinates, null);
            MapElement dynamic = gameGrid[(int) Layer.Dynamic][column, row];
            MapEntity entity = (MapEntity) gameGrid[(int) Layer.Entities][column, row];
            MapTile collide = (MapTile) gameGrid[(int) Layer.Collide][column, row];
            MapTile terrain = (MapTile) gameGrid[(int) Layer.Terrain][column, row];

            return new MapSlice(unit, dynamic, entity, collide, terrain);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MapElement tile in gameGrid[(int) Layer.Terrain])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            foreach (MapElement tile in gameGrid[(int) Layer.Collide])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            foreach (MapElement tile in gameGrid[(int) Layer.Entities])
            {
                if (tile != null)
                    tile.Draw(spriteBatch);
            }

            foreach (MapElement tile in gameGrid[(int) Layer.Dynamic])
            {
                if (tile != null)
                    tile.Draw(spriteBatch, new Color(255, 255, 255, 180));
            }

            foreach (GameUnit unit in GameContext.Units)
            {
                if (unit.MapEntity != null)
                {
                    unit.MapEntity.Draw(spriteBatch);
                }
            }

            //Draw map cursor
            mapCursor.Draw(spriteBatch);
        }
    }
}
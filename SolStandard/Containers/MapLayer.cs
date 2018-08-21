using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers
{
    public class MapLayer
    {
        private readonly List<MapElement[,]> gameGrid;
        private readonly MapCursor mapCursor;

        public MapLayer(List<MapElement[,]> gameGrid, ITexture2D cursorTexture)
        {
            this.gameGrid = gameGrid;
            mapCursor = BuildMapCursor(cursorTexture);
        }

        private MapCursor BuildMapCursor(ITexture2D cursorTexture)
        {
            TileCell cursorCell = new TileCell(cursorTexture, GameDriver.CellSize, 1);
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

        public MapSlice GetMapSliceAtCursor()
        {
            int column = (int) mapCursor.MapCoordinates.X;
            int row = (int) mapCursor.MapCoordinates.Y;

            MapEntity unit = (MapEntity) gameGrid[(int) Layer.Units][column, row];
            MapEntity entity = (MapEntity) gameGrid[(int) Layer.Entities][column, row];
            MapTile collide = (MapTile) gameGrid[(int) Layer.Collide][column, row];
            MapTile terrain = (MapTile) gameGrid[(int) Layer.Terrain][column, row];

            return new MapSlice(unit, entity, collide, terrain);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw tiles in Map Grid
            foreach (MapElement[,] layer in gameGrid)
            {
                foreach (MapElement tile in layer)
                {
                    if (tile != null)
                        tile.Draw(spriteBatch);
                }
            }

            //Draw map cursor
            mapCursor.Draw(spriteBatch);
        }
    }
}
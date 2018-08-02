using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using SolStandard.Map.Objects;
using SolStandard.Map.Objects.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Map
{
    public class MapContainer
    {
        private readonly List<MapObject[,]> gameGrid;
        private readonly MapCursor mapCursor;

        public MapContainer(List<MapObject[,]> gameGrid, ITexture2D cursorTexture)
        {
            this.gameGrid = gameGrid;
            this.mapCursor = BuildMapCursor(cursorTexture);
        }

        private MapCursor BuildMapCursor(ITexture2D cursorTexture)
        {
            TileCell cursorCell = new TileCell(cursorTexture, GameDriver.CELL_SIZE, 1);
            Vector2 cursorStartPosition = new Vector2(0);
            return new MapCursor(cursorCell, cursorStartPosition, MapSize());
        }

        public MapCursor GetMapCursor()
        {
            return mapCursor;
        }

        public ReadOnlyCollection<MapObject[,]> GetGameGrid()
        {
            return gameGrid.AsReadOnly();
        }

        public Vector2 MapSize()
        {
            return new Vector2(gameGrid[0].GetLength(0), gameGrid[0].GetLength(1));
        }
    }
}
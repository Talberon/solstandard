using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Map.Objects;
using SolStandard.Map.Objects.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers
{
    public class MapLayer
    {
        private readonly List<MapObject[,]> gameGrid;
        private readonly MapCursor mapCursor;

        public MapLayer(List<MapObject[,]> gameGrid, ITexture2D cursorTexture)
        {
            this.gameGrid = gameGrid;
            mapCursor = BuildMapCursor(cursorTexture);
        }

        private MapCursor BuildMapCursor(ITexture2D cursorTexture)
        {
            TileCell cursorCell = new TileCell(cursorTexture, GameDriver.CellSize, 1);
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
        
        public void Draw(SpriteBatch spriteBatch)
        {            
            //Draw tiles in Map Grid
            foreach (MapObject[,] layer in gameGrid)
            {
                foreach (MapObject tile in layer)
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
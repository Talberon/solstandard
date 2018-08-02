using System;
using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Map.Objects.Cursor
{
    public class MapCursor : MapObject
    {
        public enum CursorDirection
        {
            Up,
            Right,
            Down,
            Left
        }

        private readonly Vector2 mapSize;

        public MapCursor(TileCell tileCell, Vector2 mapCoordinates, Vector2 mapSize)
        {
            this.tileCell = tileCell;
            this.mapCoordinates = mapCoordinates;
            this.mapSize = mapSize;
        }

        public void MoveCursorInDirection(CursorDirection direction)
        {
            switch (direction)
            {
                case CursorDirection.Down:
                    mapCoordinates.Y++;
                    break;
                case CursorDirection.Right:
                    mapCoordinates.X++;
                    break;
                case CursorDirection.Up:
                    mapCoordinates.Y--;
                    break;
                case CursorDirection.Left:
                    mapCoordinates.X--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            PreventCursorLeavingMapBounds();
        }

        public Vector2 GetMapCoordinates()
        {
            return mapCoordinates;
        }

        private void PreventCursorLeavingMapBounds()
        {
            if (mapCoordinates.X < 0)
            {
                mapCoordinates.X = 0;
            }

            if (mapCoordinates.X >= mapSize.X)
            {
                mapCoordinates.X = mapSize.X - 1;
            }

            if (mapCoordinates.Y < 0)
            {
                mapCoordinates.Y = 0;
            }

            if (mapCoordinates.Y >= mapSize.Y)
            {
                mapCoordinates.Y = mapSize.Y - 1;
            }
        }

        public override string ToString()
        {
            return "Cursor: {" + tileCell.ToString() + "}";
        }
    }
}
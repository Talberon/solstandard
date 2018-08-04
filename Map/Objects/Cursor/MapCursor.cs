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

        public MapCursor(TileCell sprite, Vector2 mapCoordinates, Vector2 mapSize)
        {
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
            this.mapSize = mapSize;
        }

        public void MoveCursorInDirection(CursorDirection direction)
        {
            switch (direction)
            {
                case CursorDirection.Down:
                    MapCoordinates.Y++;
                    break;
                case CursorDirection.Right:
                    MapCoordinates.X++;
                    break;
                case CursorDirection.Up:
                    MapCoordinates.Y--;
                    break;
                case CursorDirection.Left:
                    MapCoordinates.X--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            PreventCursorLeavingMapBounds();
        }

        public Vector2 GetMapCoordinates()
        {
            return MapCoordinates;
        }

        private void PreventCursorLeavingMapBounds()
        {
            if (MapCoordinates.X < 0)
            {
                MapCoordinates.X = 0;
            }

            if (MapCoordinates.X >= mapSize.X)
            {
                MapCoordinates.X = mapSize.X - 1;
            }

            if (MapCoordinates.Y < 0)
            {
                MapCoordinates.Y = 0;
            }

            if (MapCoordinates.Y >= mapSize.Y)
            {
                MapCoordinates.Y = mapSize.Y - 1;
            }
        }

        public override string ToString()
        {
            return "Cursor: {" + Sprite + "}";
        }
    }
}
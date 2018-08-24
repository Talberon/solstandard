using System;
using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Map.Elements.Cursor
{
    public class MapCursor : MapElement
    {
        private readonly Vector2 mapSize;

        public MapCursor(IRenderable sprite, Vector2 mapCoordinates, Vector2 mapSize)
        {
            Sprite = sprite;
            MapCoordinates = mapCoordinates;
            this.mapSize = mapSize;
        }

        public void MoveCursorInDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    MapCoordinates = new Vector2(MapCoordinates.X, MapCoordinates.Y + 1);
                    break;
                case Direction.Right:
                    MapCoordinates = new Vector2(MapCoordinates.X + 1, MapCoordinates.Y);
                    break;
                case Direction.Up:
                    MapCoordinates = new Vector2(MapCoordinates.X, MapCoordinates.Y - 1);
                    break;
                case Direction.Left:
                    MapCoordinates = new Vector2(MapCoordinates.X - 1, MapCoordinates.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            PreventCursorLeavingMapBounds();
        }

        private void PreventCursorLeavingMapBounds()
        {
            if (MapCoordinates.X < 0)
            {
                MapCoordinates = new Vector2(0, MapCoordinates.Y);
            }

            if (MapCoordinates.X >= mapSize.X)
            {
                MapCoordinates = new Vector2(mapSize.X - 1, MapCoordinates.Y);
            }

            if (MapCoordinates.Y < 0)
            {
                MapCoordinates = new Vector2(MapCoordinates.X, 0);
            }

            if (MapCoordinates.Y >= mapSize.Y)
            {
                MapCoordinates = new Vector2(MapCoordinates.X, mapSize.Y - 1);
            }
        }

        public override string ToString()
        {
            return "Cursor: {" + Sprite + "}";
        }
    }
}
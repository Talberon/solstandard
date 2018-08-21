using System;
using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Map.Elements.Cursor
{
    public class MapCursor : MapElement
    {
        public enum CursorDirection
        {
            Up,
            Right,
            Down,
            Left
        }

        private readonly Vector2 mapSize;

        public MapCursor(IRenderable sprite, Vector2 mapCoordinates, Vector2 mapSize)
        {
            Sprite = sprite;
            base.MapCoordinates = mapCoordinates;
            this.mapSize = mapSize;
        }

        public void MoveCursorInDirection(CursorDirection direction)
        {
            switch (direction)
            {
                case CursorDirection.Down:
                    base.MapCoordinates.Y++;
                    break;
                case CursorDirection.Right:
                    base.MapCoordinates.X++;
                    break;
                case CursorDirection.Up:
                    base.MapCoordinates.Y--;
                    break;
                case CursorDirection.Left:
                    base.MapCoordinates.X--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }

            PreventCursorLeavingMapBounds();
        }

        public Vector2 MapCoordinates
        {
            get { return base.MapCoordinates; }
        }

        private void PreventCursorLeavingMapBounds()
        {
            if (base.MapCoordinates.X < 0)
            {
                base.MapCoordinates.X = 0;
            }

            if (base.MapCoordinates.X >= mapSize.X)
            {
                base.MapCoordinates.X = mapSize.X - 1;
            }

            if (base.MapCoordinates.Y < 0)
            {
                base.MapCoordinates.Y = 0;
            }

            if (base.MapCoordinates.Y >= mapSize.Y)
            {
                base.MapCoordinates.Y = mapSize.Y - 1;
            }
        }

        public override string ToString()
        {
            return "Cursor: {" + Sprite + "}";
        }
        
    }
}
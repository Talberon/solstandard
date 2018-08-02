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
        
        public MapCursor(TileCell tileCell, Vector2 mapCoordinates)
        {
            this.tileCell = tileCell;
            this.mapCoordinates = mapCoordinates;
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
        }
        
        public override string ToString()
        {
            return "Cursor: {" + tileCell.ToString() + "}";
        }
    }
}
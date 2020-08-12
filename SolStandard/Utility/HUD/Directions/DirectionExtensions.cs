using System;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.HUD.Directions
{
    public static class DirectionExtensions
    {
        public static Vector2 ToVector(this CardinalDirection me)
        {
            return me switch
            {
                CardinalDirection.North => Vector2.UnitY.Inverted(),
                CardinalDirection.South => Vector2.UnitY,
                CardinalDirection.West => Vector2.UnitX.Inverted(),
                CardinalDirection.East => Vector2.UnitX,
                _ => Vector2.Zero
            };
        }

        public static Vector2 ToVector(this IntercardinalDirection me)
        {
            return me switch
            {
                IntercardinalDirection.North => new Vector2(0, -1),
                IntercardinalDirection.South => new Vector2(0, 1),
                IntercardinalDirection.East => new Vector2(1, 0),
                IntercardinalDirection.West => new Vector2(-1, 0),
                IntercardinalDirection.NorthEast => new Vector2(1, -1),
                IntercardinalDirection.NorthWest => new Vector2(-1, -1),
                IntercardinalDirection.SouthEast => new Vector2(1, 1),
                IntercardinalDirection.SouthWest => new Vector2(-1, 1),
                _ => Vector2.Zero
            };
        }

        public static CardinalDirection OppositeDirection(this CardinalDirection me)
        {
            return me switch
            {
                CardinalDirection.North => CardinalDirection.South,
                CardinalDirection.South => CardinalDirection.North,
                CardinalDirection.West => CardinalDirection.East,
                CardinalDirection.East => CardinalDirection.West,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static IntercardinalDirection OppositeDirection(this IntercardinalDirection me)
        {
            return me switch
            {
                IntercardinalDirection.North => IntercardinalDirection.South,
                IntercardinalDirection.NorthEast => IntercardinalDirection.SouthWest,
                IntercardinalDirection.East => IntercardinalDirection.West,
                IntercardinalDirection.SouthEast => IntercardinalDirection.NorthWest,
                IntercardinalDirection.South => IntercardinalDirection.North,
                IntercardinalDirection.SouthWest => IntercardinalDirection.NorthEast,
                IntercardinalDirection.West => IntercardinalDirection.East,
                IntercardinalDirection.NorthWest => IntercardinalDirection.NorthEast,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
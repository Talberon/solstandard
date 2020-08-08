using System;
using Microsoft.Xna.Framework;
using SolStandard.NeoUtility.Directions;

namespace SolStandard.NeoUtility.General
{
    public static class GamePhysics
    {
        public static Vector2 ApplyFriction(Vector2 currentVelocity, float friction)
        {
            if (currentVelocity == Vector2.Zero || friction < 0.001f) return currentVelocity;

            Vector2 updatedVelocity = currentVelocity;

            if (updatedVelocity.X > 0)
            {
                if (updatedVelocity.X < friction) updatedVelocity.X = 0;
                else updatedVelocity.X -= friction;
            }
            else if (updatedVelocity.X < 0)
            {
                if (updatedVelocity.X > -friction) updatedVelocity.X = 0;
                else updatedVelocity.X += friction;
            }

            if (updatedVelocity.Y > 0)
            {
                if (updatedVelocity.Y < friction) updatedVelocity.Y = 0;
                else updatedVelocity.Y -= friction;
            }
            else if (updatedVelocity.Y < 0)
            {
                if (updatedVelocity.Y > -friction) updatedVelocity.Y = 0;
                else updatedVelocity.Y += friction;
            }

            return updatedVelocity;
        }
        
        public static Vector2 VelocityForDirection(CardinalDirection direction, float speed)
        {
            return direction switch
            {
                CardinalDirection.North => new Vector2(0, -speed),
                CardinalDirection.South => new Vector2(0, speed),
                CardinalDirection.West => new Vector2(-speed, 0),
                CardinalDirection.East => new Vector2(speed, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static float GetDegreesForRadians(this float radians)
        {
            return radians * (180f / 3.14f);
        }
    }
}
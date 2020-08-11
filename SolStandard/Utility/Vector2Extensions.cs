using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace SolStandard.Utility
{
    public static class Vector2Extensions
    {
        public static Vector2 ToVector2(this Point2 me)
        {
            return new Vector2(me.X, me.Y);
        }

        public static Vector2 WithoutPrecision(this Vector2 me)
        {
            (float x, float y) = me;
            return new Vector2((int) x, (int) y);
        }

        public static void AddWithCap(this ref float me, float amountToAdd, float maxValue)
        {
            me = (Math.Abs(amountToAdd + me) >= Math.Abs(maxValue)) ? maxValue : me + amountToAdd;
        }

        public static bool HasGreaterAbsoluteValueThan(this Vector2 me, Vector2 them, float tolerance)
        {
            float mySum = me.AbsX() + me.AbsY();
            float theirSum = them.AbsX() + them.AbsY();

            return mySum > theirSum + tolerance;
        }

        public static bool IsNotZero(this Vector2 me, float tolerance = 0.001f)
        {
            return me.HasGreaterAbsoluteValueThan(Vector2.Zero, tolerance);
        }

        public static float AbsX(this Vector2 me)
        {
            return Math.Abs(me.X);
        }

        public static float AbsY(this Vector2 me)
        {
            return Math.Abs(me.Y);
        }

        public static Vector2 Inverted(this Vector2 me)
        {
            (float x, float y) = me;
            return new Vector2(-x, -y);
        }

        public static Vector2 Absolute(this Vector2 me)
        {
            (float x, float y) = me;
            return new Vector2(Math.Abs(x), Math.Abs(y));
        }

        public static float GetAngle(this Vector2 me)
        {
            (float x, float y) = me;
            return (float) Math.Atan2(y, x);
        }

        public static Vector2 WithVariance(this Vector2 me, float maxVariance)
        {
            (float x, float y) = me;

            return new Vector2(
                x + GameDriver.Random.NextSingle(-maxVariance, maxVariance),
                y + GameDriver.Random.NextSingle(-maxVariance, maxVariance)
            );
        }

        public static float DistanceFrom(this Vector2 me, Vector2 target)
        {
            //Pythagorean theorem: Find the hypotenuse (a^2 + b^2 = c^2)
            (float a, float b) = (me - target).Absolute();
            return (float) Math.Sqrt((a * a) + (b * b));
        }
    }
}
using System;

namespace SolStandard.Utility
{
    public static class MathUtils
    {
        public static int Ceiling(float value)
        {
            return (int) Math.Ceiling(value);
        }

        public static float AsymptoticAverage(float origin, float target, float speed = 1f, float tolerance = 0.000001f)
        {
            float asymptoticAverage = origin + (target - origin) * .1f * speed;

            return (Math.Abs(target - asymptoticAverage) > tolerance) ? asymptoticAverage : target;
        }
    }
}
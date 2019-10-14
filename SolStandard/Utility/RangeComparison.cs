using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility
{
    public static class RangeComparison
    {
        /// <summary>
        /// Check if two points are within range on a square grid where each int is one cardinal direction point from the origin.
        /// 
        /// NOTE: This assumes that there are no concentric rings in the range.
        /// 
        /// NOTE: This does not account for pathing; this is a pure numbers comparison as if no obstacles would obstruct the range.
        /// </summary>
        /// <param name="originPosition"></param>
        /// <param name="rangeFromOrigin"></param>
        /// <param name="targetPosition"></param>
        /// <returns>True if the target is within range.</returns>
        public static bool TargetIsWithinRangeOfOrigin(Vector2 originPosition, IEnumerable<int> rangeFromOrigin,
            Vector2 targetPosition)
        {
            Vector2 adjustedTarget = targetPosition - originPosition;
            (float targetX, float targetY) = adjustedTarget;

            int distanceFromOrigin = Math.Abs(Convert.ToInt32(targetX)) + Math.Abs(Convert.ToInt32(targetY));

            return rangeFromOrigin.Any(range => distanceFromOrigin == range);
        }
    }
}
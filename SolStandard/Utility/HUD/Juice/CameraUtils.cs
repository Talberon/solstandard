using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Collections;

namespace SolStandard.Utility.HUD.Juice
{
    public static class CameraUtils
    {
        public static Vector2 CenterOfPoints(params Vector2[] points)
        {
            return CenterOfPoints(points as IEnumerable<Vector2>);
        }

        public static Vector2 CenterOfPoints(IEnumerable<Vector2> points)
        {
            Vector2[] enumerable = points as Vector2[] ?? points.ToArray();

            if (enumerable.IsEmpty()) return Vector2.Zero;

            //Get lowest and highest X
            //Subtract both by the lowest
            //Divide reduced highest by 2
            float lowestX = enumerable.Min(point => point.X);
            float highestX = enumerable.Max(p => p.X);

            float halfHighestXReduced = (highestX - lowestX) / 2;

            //Get lowest and highest Y
            //subtract both by lowest
            //Divide reduced highest by 2
            float lowestY = enumerable.Min(p => p.Y);
            float highestY = enumerable.Max(p => p.Y);

            float halfHighestYReduced = (highestY - lowestY) / 2;

            return new Vector2(halfHighestXReduced + lowestX, halfHighestYReduced + lowestY);
        }

        public static float DynamicZoom(Vector2 centerPoint, IEnumerable<Vector2> playerPositions,
            Vector2 cameraZoomBuffer, float maxZoom)
        {
            float furthestY = 0;
            float furthestX = 0;

            foreach (Vector2 position in playerPositions)
            {
                (float relativeX, float relativeY) = position - centerPoint;

                if (Math.Abs(relativeX) > furthestX) furthestX = Math.Abs(relativeX);
                if (Math.Abs(relativeY) > furthestY) furthestY = Math.Abs(relativeY);
            }

            (float halfX, float halfY) = (GameDriver.RenderResolution - cameraZoomBuffer) / 2;

            float xRatio = halfX / furthestX;
            float yRatio = halfY / furthestY;

            float smallerRatio = (xRatio < yRatio) ? xRatio : yRatio;

            return (smallerRatio < maxZoom) ? smallerRatio : maxZoom;
        }
    }
}
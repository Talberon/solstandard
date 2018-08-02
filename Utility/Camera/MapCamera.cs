using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using SolStandard.Map.Objects.Cursor;

namespace SolStandard.Utility.Camera
{
    public enum CameraDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    public class MapCamera
    {
        private const int HorizontalThreshold = (7 * GameDriver.CELL_SIZE);
        private const int VerticalThreshold = (5 * GameDriver.CELL_SIZE);
        private float currentZoom;

        private Vector2 currentCamPosition;

        private Vector2 targetCamPosition;
        private readonly float cameraPanRate;

        public MapCamera(float cameraPanRate)
        {
            this.currentCamPosition = new Vector2(0);
            this.targetCamPosition = new Vector2(0);
            this.currentZoom = 1;
            this.cameraPanRate = cameraPanRate;
        }

        public void SetTargetCameraPosition(Vector2 targetPosition)
        {
            this.targetCamPosition = targetPosition;
        }

        public void SetCameraZoom(float zoom)
        {
            this.currentZoom = zoom;
        }

        public void MoveCameraInDirection(CameraDirection direction)
        {
            switch (direction)
            {
                case CameraDirection.Down:
                    targetCamPosition.Y -= cameraPanRate;
                    break;
                case CameraDirection.Right:
                    targetCamPosition.X -= cameraPanRate;
                    break;
                case CameraDirection.Up:
                    targetCamPosition.Y += cameraPanRate;
                    break;
                case CameraDirection.Left:
                    targetCamPosition.X += cameraPanRate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        public void PanCameraToTarget()
        {
            if (currentCamPosition.X < targetCamPosition.X)
            {
                if (currentCamPosition.X + cameraPanRate > targetCamPosition.X)
                {
                    currentCamPosition.X = targetCamPosition.X;
                }
                else
                {
                    currentCamPosition.X += cameraPanRate;
                }
            }

            if (currentCamPosition.X > targetCamPosition.X)
            {
                if (currentCamPosition.X - cameraPanRate < targetCamPosition.X)
                {
                    currentCamPosition.X = targetCamPosition.X;
                }
                else
                {
                    currentCamPosition.X -= cameraPanRate;
                }
            }

            if (currentCamPosition.Y < targetCamPosition.Y)
            {
                if (currentCamPosition.Y + cameraPanRate > targetCamPosition.Y)
                {
                    currentCamPosition.Y = targetCamPosition.Y;
                }
                else
                {
                    currentCamPosition.Y += cameraPanRate;
                }
            }

            if (currentCamPosition.Y > targetCamPosition.Y)
            {
                if (currentCamPosition.Y - cameraPanRate < targetCamPosition.Y)
                {
                    currentCamPosition.Y = targetCamPosition.Y;
                }
                else
                {
                    currentCamPosition.Y -= cameraPanRate;
                }
            }
        }

        public Matrix GetCameraMatrix()
        {
            return Matrix.CreateTranslation(currentCamPosition.X, currentCamPosition.Y, 0) *
                   Matrix.CreateScale(new Vector3(currentZoom, currentZoom, 1));
        }

        public void CorrectCameraToCursor(MapCursor cursor, Vector2 screenDimensions, Vector2 mapSize)
        {
            if (currentCamPosition != targetCamPosition) return;

            if (cursor.GetMapCoordinates().X * GameDriver.CELL_SIZE < GetWestBound(currentCamPosition.X))
            {
                MoveCameraInDirection(CameraDirection.Left);
            }

            if (cursor.GetMapCoordinates().X * GameDriver.CELL_SIZE >
                GetEastBound(screenDimensions.X, currentCamPosition.X))
            {
                MoveCameraInDirection(CameraDirection.Right);
            }

            if (cursor.GetMapCoordinates().Y * GameDriver.CELL_SIZE < GetNorthBound(currentCamPosition.Y))
            {
                MoveCameraInDirection(CameraDirection.Up);
            }

            if (cursor.GetMapCoordinates().Y * GameDriver.CELL_SIZE >
                GetSouthBound(screenDimensions.Y, currentCamPosition.Y))
            {
                MoveCameraInDirection(CameraDirection.Down);
            }


            CorrectCameraToMap(screenDimensions, mapSize);
        }

        private float GetWestBound(float cursorX)
        {
            return (0 + (HorizontalThreshold + GameDriver.CELL_SIZE)) / currentZoom - cursorX;
        }

        private float GetEastBound(float borderX, float cursorX)
        {
            return (borderX - HorizontalThreshold) / currentZoom - cursorX;
        }

        private float GetNorthBound(float cursorY)
        {
            return (0 + (VerticalThreshold + GameDriver.CELL_SIZE)) / currentZoom - cursorY;
        }

        private float GetSouthBound(float borderY, float cursorY)
        {
            return (borderY - VerticalThreshold) / currentZoom - cursorY;
        }

        private void CorrectCameraToMap(Vector2 screenSize, Vector2 mapSize)
        {
            //Left Edge
            if (targetCamPosition.X > 0)
                targetCamPosition.X = 0;
            //Top Edge
            if (targetCamPosition.Y > 0)
                targetCamPosition.Y = 0;
            //Right Edge
            if (targetCamPosition.X * currentZoom < ((-1) * mapSize.X * GameDriver.CELL_SIZE) * currentZoom + screenSize.X)
                targetCamPosition.X = (((-1) * mapSize.X * GameDriver.CELL_SIZE) * currentZoom + screenSize.X) / currentZoom;
            //Bottom Edge
            if (targetCamPosition.Y * currentZoom < ((-1) * mapSize.Y * GameDriver.CELL_SIZE) * currentZoom + screenSize.Y)
                targetCamPosition.Y = (((-1) * mapSize.Y * GameDriver.CELL_SIZE) * currentZoom + screenSize.Y) / currentZoom;
        }
    }
}
using System;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements.Cursor;

namespace SolStandard.Map.Camera
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
        private const int HorizontalThreshold = (7 * GameDriver.CellSize);
        private const int TopThreshold = (4 * GameDriver.CellSize);
        private const int BottomThreshold = (7 * GameDriver.CellSize);
        private const int DefaultZoomLevel = 2;

        private float currentZoom;
        private float targetZoom;
        private readonly float zoomTransitionRate;


        private Vector2 currentCamPosition;
        private Vector2 targetCamPosition;
        private readonly float cameraPanRate;

        public MapCamera(float cameraPanRate, float zoomTransitionRate)
        {
            currentCamPosition = new Vector2(0);
            targetCamPosition = new Vector2(0);
            currentZoom = DefaultZoomLevel;
            targetZoom = currentZoom;
            this.cameraPanRate = cameraPanRate;
            this.zoomTransitionRate = zoomTransitionRate;
        }

        

        public void SetTargetCameraPosition(Vector2 targetPosition)
        {
            targetCamPosition = targetPosition;
        }

        public void SetCameraZoom(float zoom)
        {
            targetZoom = zoom;
        }

        public void IncreaseZoom(float zoomChange)
        {
            const float maximumZoom = 3.0f;
            if (targetZoom < maximumZoom) targetZoom += zoomChange;
        }

        public void DecreaseZoom(float zoomChange)
        {
            const float minimumZoom = 1.0f;
            if (targetZoom > minimumZoom) targetZoom -= zoomChange;
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

        public void UpdateEveryFrame()
        {
            UpdateZoomLevel();
            PanCameraToTarget();
        }

        private void UpdateZoomLevel()
        {
            //Too big; zoom out
            if (currentZoom > targetZoom)
            {
                if (currentZoom - zoomTransitionRate < targetZoom) return;
                currentZoom -= zoomTransitionRate;
            }

            //Too small; zoom in
            if (currentZoom < targetZoom)
            {
                if (currentZoom + zoomTransitionRate > targetZoom) return;
                currentZoom += zoomTransitionRate;
            }
        }

        private void PanCameraToTarget()
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

        public Matrix CameraMatrix
        {
            get
            {
                return Matrix.CreateTranslation(currentCamPosition.X, currentCamPosition.Y, 0) *
                       Matrix.CreateScale(new Vector3(currentZoom, currentZoom, 1));
            }
        }

        public void CorrectCameraToCursor(MapCursor cursor, Vector2 mapSize)
        {
            if (currentCamPosition != targetCamPosition) return;

            if (cursor.MapCoordinates.X * GameDriver.CellSize < GetWestBound(currentCamPosition.X))
            {
                MoveCameraInDirection(CameraDirection.Left);
            }

            if (cursor.MapCoordinates.X * GameDriver.CellSize >
                GetEastBound(GameDriver.ScreenSize.X, currentCamPosition.X))
            {
                MoveCameraInDirection(CameraDirection.Right);
            }

            if (cursor.MapCoordinates.Y * GameDriver.CellSize < GetNorthBound(currentCamPosition.Y))
            {
                MoveCameraInDirection(CameraDirection.Up);
            }

            if (cursor.MapCoordinates.Y * GameDriver.CellSize >
                GetSouthBound(GameDriver.ScreenSize.Y, currentCamPosition.Y))
            {
                MoveCameraInDirection(CameraDirection.Down);
            }


            CorrectCameraToMap(GameDriver.ScreenSize, mapSize);
        }

        private float GetWestBound(float cursorX)
        {
            return (0 + (HorizontalThreshold + GameDriver.CellSize)) / currentZoom - cursorX;
        }

        private float GetEastBound(float borderX, float cursorX)
        {
            return (borderX - HorizontalThreshold) / currentZoom - cursorX;
        }

        private float GetNorthBound(float cursorY)
        {
            return (0 + (TopThreshold + GameDriver.CellSize)) / currentZoom - cursorY;
        }

        private float GetSouthBound(float borderY, float cursorY)
        {
            return (borderY - BottomThreshold) / currentZoom - cursorY;
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
            if (targetCamPosition.X * currentZoom <
                ((-1) * mapSize.X * GameDriver.CellSize) * currentZoom + screenSize.X)
                targetCamPosition.X = (((-1) * mapSize.X * GameDriver.CellSize) * currentZoom + screenSize.X) /
                                      currentZoom;
            //Bottom Edge
            if (targetCamPosition.Y * currentZoom <
                ((-1) * mapSize.Y * GameDriver.CellSize) * currentZoom + screenSize.Y)
                targetCamPosition.Y = (((-1) * mapSize.Y * GameDriver.CellSize) * currentZoom + screenSize.Y) /
                                      currentZoom;
        }
    }
}
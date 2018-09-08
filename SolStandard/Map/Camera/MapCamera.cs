using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
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
        private const float MaximumZoom = 4.0f;
        private const float MinimumZoom = 2.0f;

        private float currentZoom;
        private float targetZoom;
        private readonly float zoomRate;

        private Vector2 currentPosition;
        private Vector2 targetPosition;
        private readonly float panRate;

        private bool centeringOnPoint;

        public MapCamera(float panRate, float zoomRate)
        {
            currentPosition = new Vector2(0);
            targetPosition = new Vector2(0);
            currentZoom = DefaultZoomLevel;
            targetZoom = currentZoom;
            centeringOnPoint = false;
            this.panRate = panRate;
            this.zoomRate = zoomRate;
        }

        public Matrix CameraMatrix
        {
            get
            {
                return Matrix.CreateTranslation(currentPosition.X, currentPosition.Y, 0) *
                       Matrix.CreateScale(new Vector3(currentZoom, currentZoom, 1));
            }
        }

        private Vector2 CursorScreenCoordinates
        {
            get { return ((MapContainer.MapCursor.PixelCoordinates + targetPosition) * targetZoom); }
        }

        public void IncreaseZoom(float newTargetZoom)
        {
            if (targetZoom < MaximumZoom) targetZoom += newTargetZoom;
        }

        public void DecreaseZoom(float newTargetZoom)
        {
            if (targetZoom > MinimumZoom) targetZoom -= newTargetZoom;
        }

        public float CurrentZoom
        {
            get { return currentZoom; }
        }

        public void SetCameraZoom(float zoom)
        {
            targetZoom = zoom;
            currentZoom = targetZoom;
        }

        public void ZoomToCursor(float zoomLevel)
        {
            targetZoom = zoomLevel;
            centeringOnPoint = true;
        }

        public void UpdateEveryFrame()
        {
            if (centeringOnPoint)
            {
                CenterCursor();
            }

            if (targetPosition == currentPosition && Math.Abs(targetZoom - currentZoom) < 0.01)
            {
                centeringOnPoint = false;
            }

            UpdateZoomLevel();
            PanCameraToTarget();
            CorrectCameraToCursor(MapContainer.MapCursor, MapContainer.MapGridSize);
        }

        private void CenterCursor()
        {
            CenterToPoint(MapContainer.MapCursor.CenterPixelPoint);
        }

        private void CenterToPoint(Vector2 cursorCenter)
        {
            Vector2 screenCenter = GameDriver.ScreenSize / 2;

            targetPosition = Vector2.Negate(cursorCenter);
            targetPosition += screenCenter / currentZoom;

            Trace.WriteLine("Camera:" + targetPosition);
            Trace.WriteLine("Cursor:" + CursorScreenCoordinates);
            Trace.WriteLine("Cursor:" + MapContainer.MapCursor.PixelCoordinates);
        }

        private void UpdateZoomLevel()
        {
            //Too big; zoom out
            if (currentZoom > targetZoom)
            {
                if (currentZoom - zoomRate < targetZoom)
                {
                    currentZoom = targetZoom;
                    return;
                }
                currentZoom -= zoomRate;
            }

            //Too small; zoom in
            if (currentZoom < targetZoom)
            {
                if (currentZoom + zoomRate > targetZoom)
                {
                    currentZoom = targetZoom;
                    return;
                }
                currentZoom += zoomRate;
            }
        }

        private void PanCameraToTarget()
        {
            if (currentPosition.X < targetPosition.X)
            {
                if (currentPosition.X + panRate > targetPosition.X)
                {
                    currentPosition.X = targetPosition.X;
                }
                else
                {
                    currentPosition.X += panRate;
                }
            }

            if (currentPosition.X > targetPosition.X)
            {
                if (currentPosition.X - panRate < targetPosition.X)
                {
                    currentPosition.X = targetPosition.X;
                }
                else
                {
                    currentPosition.X -= panRate;
                }
            }

            if (currentPosition.Y < targetPosition.Y)
            {
                if (currentPosition.Y + panRate > targetPosition.Y)
                {
                    currentPosition.Y = targetPosition.Y;
                }
                else
                {
                    currentPosition.Y += panRate;
                }
            }

            if (currentPosition.Y > targetPosition.Y)
            {
                if (currentPosition.Y - panRate < targetPosition.Y)
                {
                    currentPosition.Y = targetPosition.Y;
                }
                else
                {
                    currentPosition.Y -= panRate;
                }
            }
        }


        public void MoveCameraInDirection(CameraDirection direction)
        {
            switch (direction)
            {
                case CameraDirection.Down:
                    targetPosition.Y -= panRate;
                    break;
                case CameraDirection.Right:
                    targetPosition.X -= panRate;
                    break;
                case CameraDirection.Up:
                    targetPosition.Y += panRate;
                    break;
                case CameraDirection.Left:
                    targetPosition.X += panRate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        public void CorrectCameraToCursor(MapCursor cursor, Vector2 mapSize)
        {
            if (currentPosition != targetPosition) return;

            if (cursor.MapCoordinates.X * GameDriver.CellSize < GetWestBound(currentPosition.X))
            {
                MoveCameraInDirection(CameraDirection.Left);
            }

            if (cursor.MapCoordinates.X * GameDriver.CellSize >
                GetEastBound(GameDriver.ScreenSize.X, currentPosition.X))
            {
                MoveCameraInDirection(CameraDirection.Right);
            }

            if (cursor.MapCoordinates.Y * GameDriver.CellSize < GetNorthBound(currentPosition.Y))
            {
                MoveCameraInDirection(CameraDirection.Up);
            }

            if (cursor.MapCoordinates.Y * GameDriver.CellSize >
                GetSouthBound(GameDriver.ScreenSize.Y, currentPosition.Y))
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
            if (targetPosition.X > 0)
                targetPosition.X = 0;
            //Top Edge
            if (targetPosition.Y > 0)
                targetPosition.Y = 0;
            //Right Edge
            if (targetPosition.X * currentZoom <
                ((-1) * mapSize.X * GameDriver.CellSize) * currentZoom + screenSize.X)
                targetPosition.X = (((-1) * mapSize.X * GameDriver.CellSize) * currentZoom + screenSize.X) /
                                   currentZoom;
            //Bottom Edge
            if (targetPosition.Y * currentZoom <
                ((-1) * mapSize.Y * GameDriver.CellSize) * currentZoom + screenSize.Y)
                targetPosition.Y = (((-1) * mapSize.Y * GameDriver.CellSize) * currentZoom + screenSize.Y) /
                                   currentZoom;
        }
    }
}
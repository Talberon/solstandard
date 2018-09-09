using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers;

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
        private const int TopThreshold = 150;
        private const int HorizontalThreshold = 200;
        private const int BottomThreshold = 200;
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

        private Vector2 MapCursorScreenCoordinates
        {
            get { return ((MapContainer.MapCursor.PixelCoordinates + targetPosition) * targetZoom); }
        }

        public void IncreaseZoom(float newTargetZoom)
        {
            if (targetZoom < MaximumZoom)
            {
                ZoomToCursor(targetZoom + newTargetZoom);
            }
        }

        public void DecreaseZoom(float newTargetZoom)
        {
            if (targetZoom > MinimumZoom)
            {
                ZoomToCursor(targetZoom - newTargetZoom);
            }
        }

        public float CurrentZoom
        {
            get { return currentZoom; }
        }

        public void SetCameraZoom(float zoom)
        {
            targetZoom = zoom;
        }

        public void ZoomToCursor(float zoomLevel)
        {
            targetZoom = zoomLevel;
            centeringOnPoint = true;
        }

        public void UpdateEveryFrame()
        {
            if (targetPosition == currentPosition && Math.Abs(targetZoom - currentZoom) < 0.01)
            {
                centeringOnPoint = false;
            }

            if (centeringOnPoint)
            {
                CenterCursor();
                PanCameraToTarget(1 / zoomRate);
            }
            else
            {
                PanCameraToTarget(panRate);
            }

            UpdateZoomLevel();
            CorrectCameraToCursor();
        }

        private void CenterCursor()
        {
            CenterToPoint(MapContainer.MapCursor.CenterPixelPoint);
        }

        private void CenterToPoint(Vector2 centerPoint)
        {
            Vector2 screenCenter = GameDriver.ScreenSize / 2;

            targetPosition = Vector2.Negate(centerPoint);
            targetPosition += screenCenter / currentZoom;

            Trace.WriteLine("Camera:" + targetPosition);
            Trace.WriteLine("TargetPoint:" + centerPoint);
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

        private void PanCameraToTarget(float panSpeed)
        {
            if (currentPosition.X < targetPosition.X)
            {
                if (currentPosition.X + panSpeed > targetPosition.X)
                {
                    currentPosition.X = targetPosition.X;
                }
                else
                {
                    currentPosition.X += panSpeed;
                }
            }

            if (currentPosition.X > targetPosition.X)
            {
                if (currentPosition.X - panSpeed < targetPosition.X)
                {
                    currentPosition.X = targetPosition.X;
                }
                else
                {
                    currentPosition.X -= panSpeed;
                }
            }

            if (currentPosition.Y < targetPosition.Y)
            {
                if (currentPosition.Y + panSpeed > targetPosition.Y)
                {
                    currentPosition.Y = targetPosition.Y;
                }
                else
                {
                    currentPosition.Y += panSpeed;
                }
            }

            if (currentPosition.Y > targetPosition.Y)
            {
                if (currentPosition.Y - panSpeed < targetPosition.Y)
                {
                    currentPosition.Y = targetPosition.Y;
                }
                else
                {
                    currentPosition.Y -= panSpeed;
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

        private void CorrectCameraToCursor()
        {
            if (MapCursorScreenCoordinates.X < WestBound)
            {
                MoveCameraInDirection(CameraDirection.Left);
            }

            if (MapCursorScreenCoordinates.X > EastBound)
            {
                MoveCameraInDirection(CameraDirection.Right);
            }

            if (MapCursorScreenCoordinates.Y < NorthBound)
            {
                MoveCameraInDirection(CameraDirection.Up);
            }

            if (MapCursorScreenCoordinates.Y > SouthBound)
            {
                MoveCameraInDirection(CameraDirection.Down);
            }


            CorrectCameraToMap();
        }

        private float WestBound
        {
            get { return 0 + HorizontalThreshold; }
        }

        private float EastBound
        {
            get
            {
                return GameDriver.ScreenSize.X - HorizontalThreshold -
                       (MapContainer.MapCursor.RenderSprite.Width * currentZoom);
            }
        }

        private float NorthBound
        {
            get { return 0 + TopThreshold; }
        }

        private float SouthBound
        {
            get
            {
                return GameDriver.ScreenSize.Y - BottomThreshold -
                       (MapContainer.MapCursor.RenderSprite.Height * currentZoom);
            }
        }

        private void CorrectCameraToMap()
        {
            CorrectPositionToMap(MapContainer.MapGridSize, currentPosition);
        }

        private void CorrectPositionToMap(Vector2 mapSize, Vector2 position)
        {
            //Left Edge
            if (position.X > 0)
            {
                currentPosition.X = 0;
                targetPosition.X = currentPosition.X;
            }

            //Top Edge
            if (position.Y > 0)
            {
                currentPosition.Y = 0;
                targetPosition.Y = currentPosition.Y;
            }

            //Right Edge
            if (position.X * currentZoom < RightEdge(mapSize))
            {
                currentPosition.X = RightEdge(mapSize) / currentZoom;
                if (targetPosition.X < currentPosition.X)
                {
                    targetPosition.X = currentPosition.X;
                }
            }

            //Bottom Edge
            if (position.Y * currentZoom < BottomEdge(mapSize))
            {
                currentPosition.Y = BottomEdge(mapSize) / currentZoom;
                if (targetPosition.Y < currentPosition.Y)
                {
                    targetPosition.Y = currentPosition.Y;
                }
            }
        }

        private float BottomEdge(Vector2 mapSize)
        {
            return (-1 * mapSize.Y * GameDriver.CellSize) * currentZoom + GameDriver.ScreenSize.Y;
        }

        private float RightEdge(Vector2 mapSize)
        {
            return (-1 * mapSize.X * GameDriver.CellSize) * currentZoom + GameDriver.ScreenSize.X;
        }
    }
}
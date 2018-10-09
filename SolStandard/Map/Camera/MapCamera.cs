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
        private const int TopThreshold = 200;
        private const int HorizontalThreshold = 200;
        private const int BottomThreshold = 300;
        private const float DefaultZoomLevel = 2;
        private const float MaximumZoom = 4.0f;
        private const float MinimumZoom = 1.0f;

        public static float CurrentZoom { get; private set; }
        public static float TargetZoom { get; private set; }
        private readonly float zoomRate;

        private static Vector2 _currentPosition;
        private static Vector2 _targetPosition;
        private readonly float panRate;

        private bool centeringOnPoint;

        public MapCamera(float panRate, float zoomRate)
        {
            _currentPosition = new Vector2(0);
            _targetPosition = new Vector2(0);
            CurrentZoom = DefaultZoomLevel;
            TargetZoom = CurrentZoom;
            centeringOnPoint = false;
            this.panRate = panRate;
            this.zoomRate = zoomRate;
        }

        public static Vector2 CurrentPosition
        {
            get { return _currentPosition; }
        }

        public static Vector2 TargetPosition
        {
            get { return _targetPosition; }
        }

        public static Matrix CameraMatrix
        {
            get
            {
                return Matrix.CreateTranslation(_currentPosition.X, _currentPosition.Y, 0) *
                       Matrix.CreateScale(new Vector3(CurrentZoom, CurrentZoom, 1));
            }
        }


        public void IncreaseZoom(float newTargetZoom)
        {
            if (TargetZoom < MaximumZoom)
            {
                ZoomToCursor(TargetZoom + newTargetZoom);
            }
        }

        public void DecreaseZoom(float newTargetZoom)
        {
            if (TargetZoom > MinimumZoom)
            {
                ZoomToCursor(TargetZoom - newTargetZoom);
            }
        }

        public void SetCameraZoom(float zoom)
        {
            TargetZoom = zoom;
        }

        public void ZoomToCursor(float zoomLevel)
        {
            TargetZoom = zoomLevel;
            centeringOnPoint = true;
        }

        public void UpdateEveryFrame()
        {
            if (_targetPosition == _currentPosition && Math.Abs(TargetZoom - CurrentZoom) < 0.01)
            {
                centeringOnPoint = false;
            }

            if (centeringOnPoint)
            {
                CenterCameraToCursor();
                PanCameraToTarget(1 / zoomRate);
            }
            else
            {
                PanCameraToTarget(panRate);
            }

            UpdateZoomLevel();
            CorrectCameraToCursor();
        }

        public void CenterCameraToCursor()
        {
            CenterCameraToPoint(MapContainer.MapCursor.CenterPixelPoint);
        }

        private void CenterCameraToPoint(Vector2 centerPoint)
        {
            Vector2 screenCenter = GameDriver.ScreenSize / 2;

            _targetPosition = Vector2.Negate(centerPoint);
            _targetPosition += screenCenter / CurrentZoom;

            Trace.WriteLine("Camera:" + _targetPosition);
            Trace.WriteLine("TargetPoint:" + centerPoint);
            Trace.WriteLine("Cursor:" + MapCursor.CurrentPixelCoordinates);
        }

        private void UpdateZoomLevel()
        {
            //Too big; zoom out
            if (CurrentZoom > TargetZoom)
            {
                if (CurrentZoom - zoomRate < TargetZoom)
                {
                    CurrentZoom = TargetZoom;
                    return;
                }

                CurrentZoom -= zoomRate;
            }

            //Too small; zoom in
            if (CurrentZoom < TargetZoom)
            {
                if (CurrentZoom + zoomRate > TargetZoom)
                {
                    CurrentZoom = TargetZoom;
                    return;
                }

                CurrentZoom += zoomRate;
            }
        }

        private void PanCameraToTarget(float panSpeed)
        {
            if (_currentPosition.X < _targetPosition.X)
            {
                if (_currentPosition.X + panSpeed > _targetPosition.X)
                {
                    _currentPosition.X = _targetPosition.X;
                }
                else
                {
                    _currentPosition.X += panSpeed;
                }
            }

            if (_currentPosition.X > _targetPosition.X)
            {
                if (_currentPosition.X - panSpeed < _targetPosition.X)
                {
                    _currentPosition.X = _targetPosition.X;
                }
                else
                {
                    _currentPosition.X -= panSpeed;
                }
            }

            if (_currentPosition.Y < _targetPosition.Y)
            {
                if (_currentPosition.Y + panSpeed > _targetPosition.Y)
                {
                    _currentPosition.Y = _targetPosition.Y;
                }
                else
                {
                    _currentPosition.Y += panSpeed;
                }
            }

            if (_currentPosition.Y > _targetPosition.Y)
            {
                if (_currentPosition.Y - panSpeed < _targetPosition.Y)
                {
                    _currentPosition.Y = _targetPosition.Y;
                }
                else
                {
                    _currentPosition.Y -= panSpeed;
                }
            }
        }

        public void MoveCameraInDirection(CameraDirection direction)
        {
            MoveCameraInDirection(direction, panRate);
        }

        public void MoveCameraInDirection(CameraDirection direction, float panRateOverride)
        {
            switch (direction)
            {
                case CameraDirection.Down:
                    _targetPosition.Y -= panRateOverride;
                    break;
                case CameraDirection.Right:
                    _targetPosition.X -= panRateOverride;
                    break;
                case CameraDirection.Up:
                    _targetPosition.Y += panRateOverride;
                    break;
                case CameraDirection.Left:
                    _targetPosition.X += panRateOverride;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        private void CorrectCameraToCursor()
        {
            if (MapCursor.ScreenCoordinates.X < WestBound)
            {
                MoveCameraInDirection(CameraDirection.Left);
            }

            if (MapCursor.ScreenCoordinates.X > EastBound)
            {
                MoveCameraInDirection(CameraDirection.Right);
            }

            if (MapCursor.ScreenCoordinates.Y < NorthBound)
            {
                MoveCameraInDirection(CameraDirection.Up);
            }

            if (MapCursor.ScreenCoordinates.Y > SouthBound)
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
                       (MapContainer.MapCursor.RenderSprite.Width * CurrentZoom);
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
                       (MapContainer.MapCursor.RenderSprite.Height * CurrentZoom);
            }
        }

        private void CorrectCameraToMap()
        {
            CorrectPositionToMap(MapContainer.MapGridSize, _currentPosition);
        }

        private void CorrectPositionToMap(Vector2 mapSize, Vector2 position)
        {
            if (CameraIsBeyondLeftEdge(position) && CameraIsBeyondRightEdge(mapSize, position))
            {
                CenterHorizontally();
            }
            else
            {
                if (CameraIsBeyondLeftEdge(position))
                {
                    _currentPosition.X = 0;
                    _targetPosition.X = _currentPosition.X;

                    //If new position would be beyond Right Edge, just center
                    if (CameraIsBeyondRightEdge(mapSize, _currentPosition)) CenterHorizontally();
                }
                else if (CameraIsBeyondRightEdge(mapSize, position))
                {
                    _currentPosition.X = RightEdge(mapSize) / CurrentZoom;
                    if (_targetPosition.X < _currentPosition.X)
                    {
                        _targetPosition.X = _currentPosition.X;
                    }

                    //If new position would be beyond Left Edge, just center
                    if (CameraIsBeyondLeftEdge(_currentPosition)) CenterHorizontally();
                }
            }

            if (CameraIsBeyondTopEdge(position) && CameraIsBeyondBottomEdge(mapSize, position))
            {
                CenterVertically();
            }
            else
            {
                if (CameraIsBeyondTopEdge(position))
                {
                    _currentPosition.Y = 0;
                    _targetPosition.Y = _currentPosition.Y;
                    //If new position would be beyond Bottom Edge, just center
                    if (CameraIsBeyondBottomEdge(mapSize, _currentPosition)) CenterVertically();
                }
                else if (CameraIsBeyondBottomEdge(mapSize, position))
                {
                    _currentPosition.Y = BottomEdge(mapSize) / CurrentZoom;
                    if (_targetPosition.Y < _currentPosition.Y)
                    {
                        _targetPosition.Y = _currentPosition.Y;
                    }

                    //If new position would be beyond Top Edge, just center
                    if (CameraIsBeyondTopEdge(_currentPosition)) CenterVertically();
                }
            }
        }

        private static void CenterVertically()
        {
            _currentPosition.Y = (GameDriver.ScreenSize.Y - MapContainer.MapScreenSizeInPixels.Y) / 2;
            _targetPosition.Y = _currentPosition.Y;
        }

        private static void CenterHorizontally()
        {
            _currentPosition.X = (GameDriver.ScreenSize.X - MapContainer.MapScreenSizeInPixels.X) / 2;
            _targetPosition.X = _currentPosition.X;
        }

        private bool CameraIsBeyondBottomEdge(Vector2 mapSize, Vector2 position)
        {
            return position.Y * CurrentZoom < BottomEdge(mapSize);
        }

        private bool CameraIsBeyondRightEdge(Vector2 mapSize, Vector2 position)
        {
            return position.X * CurrentZoom < RightEdge(mapSize);
        }

        private static bool CameraIsBeyondTopEdge(Vector2 position)
        {
            return position.Y > 0;
        }

        private static bool CameraIsBeyondLeftEdge(Vector2 position)
        {
            return position.X > 0;
        }

        private float BottomEdge(Vector2 mapSize)
        {
            return (-1 * mapSize.Y * GameDriver.CellSize) * CurrentZoom + GameDriver.ScreenSize.Y;
        }

        private float RightEdge(Vector2 mapSize)
        {
            return (-1 * mapSize.X * GameDriver.CellSize) * CurrentZoom + GameDriver.ScreenSize.X;
        }
    }
}
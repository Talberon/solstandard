using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
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
        public enum ZoomLevel
        {
            Far,
            Default,
            Close,
            Combat
        }

        private static readonly Dictionary<ZoomLevel, float> ZoomLevels = new Dictionary<ZoomLevel, float>
        {
            {ZoomLevel.Far, MinimumZoom},
            {ZoomLevel.Default, DefaultZoomLevel},
            {ZoomLevel.Close, MaximumZoom},
            {ZoomLevel.Combat, CombatZoom}
        };

        private const double FloatTolerance = 0.01;

        private const float MinimumZoom = 1.2f;
        private const float DefaultZoomLevel = 2;
        private const float MaximumZoom = 4.0f;
        private const float CombatZoom = 4;

        private const int TopCursorThreshold = 250;
        private const int HorizontalCursorThreshold = 300;
        private const int BottomCursorThreshold = 300;

        public float CurrentZoom { get; private set; }
        public float TargetZoom { get; private set; }
        private readonly float zoomRate;

        private Vector2 currentPosition;
        private Vector2 targetPosition;
        private readonly float panRate;
        private bool movingCameraToCursor;

        private bool centeringOnPoint;
        private float lastZoom;

        public MapCamera(float panRate, float zoomRate)
        {
            currentPosition = new Vector2(0);
            targetPosition = new Vector2(0);
            CurrentZoom = DefaultZoomLevel;
            TargetZoom = DefaultZoomLevel;
            lastZoom = DefaultZoomLevel;
            centeringOnPoint = false;
            this.panRate = panRate;
            this.zoomRate = zoomRate;
        }

        public Vector2 CurrentPosition
        {
            get { return currentPosition; }
        }

        public Vector2 TargetPosition
        {
            get { return targetPosition; }
        }

        public Matrix CameraMatrix
        {
            get
            {
                return Matrix.CreateTranslation(currentPosition.X, currentPosition.Y, 0) *
                       Matrix.CreateScale(new Vector3(CurrentZoom, CurrentZoom, 1));
            }
        }

        public void RevertToPreviousZoomLevel()
        {
            if (Math.Abs(TargetZoom - lastZoom) > FloatTolerance)
            {
                ZoomToCursor(lastZoom);
            }
        }

        public void SetZoomLevel(ZoomLevel zoomLevel)
        {
            ZoomToCursor(ZoomLevels[zoomLevel]);
        }

        public void ZoomIn(float newTargetZoom)
        {
            if (TargetZoom < MaximumZoom)
            {
                ZoomToCursor(TargetZoom + newTargetZoom);
            }
        }

        public void ZoomOut(float newTargetZoom)
        {
            if (TargetZoom > MinimumZoom)
            {
                ZoomToCursor(TargetZoom - newTargetZoom);
            }
        }

        private void ZoomToCursor(float zoomLevel)
        {
            if (Math.Abs(TargetZoom - zoomLevel) > FloatTolerance)
            {
                lastZoom = TargetZoom;
                TargetZoom = zoomLevel;
            }

            centeringOnPoint = true;
        }

        public void UpdateEveryFrame()
        {
            if (targetPosition == currentPosition && Math.Abs(TargetZoom - CurrentZoom) < FloatTolerance)
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
            UpdateCameraToCursor();
            CorrectCameraToMap();
        }

        public void SnapCameraCenterToCursor()
        {
            CenterCameraToPoint(GameContext.MapCursor.CenterPixelPoint);
            currentPosition = targetPosition;
        }

        public void CenterCameraToCursor()
        {
            CenterCameraToPoint(GameContext.MapCursor.CenterPixelPoint);
        }

        private void CenterCameraToPoint(Vector2 centerPoint)
        {
            Vector2 screenCenter = GameDriver.ScreenSize / 2;

            targetPosition = Vector2.Negate(centerPoint);
            targetPosition += screenCenter / CurrentZoom;

            Trace.WriteLine("Camera:" + targetPosition);
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

        private void MoveCameraInDirection(CameraDirection direction)
        {
            MoveCameraInDirection(direction, panRate);
        }

        public void MoveCameraInDirection(CameraDirection direction, float panRateOverride)
        {
            switch (direction)
            {
                case CameraDirection.Down:
                    targetPosition.Y -= panRateOverride;
                    break;
                case CameraDirection.Right:
                    targetPosition.X -= panRateOverride;
                    break;
                case CameraDirection.Up:
                    targetPosition.Y += panRateOverride;
                    break;
                case CameraDirection.Left:
                    targetPosition.X += panRateOverride;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        public void StopMovingCamera()
        {
            targetPosition = currentPosition;
        }

        public void StartMovingCameraToCursor()
        {
            movingCameraToCursor = true;
        }

        private void UpdateCameraToCursor()
        {
            if (movingCameraToCursor)
            {
                if (MapCursor.CenterCursorScreenCoordinates.X < WestBound)
                {
                    MoveCameraInDirection(CameraDirection.Left);
                }

                if (MapCursor.CenterCursorScreenCoordinates.X > EastBound)
                {
                    MoveCameraInDirection(CameraDirection.Right);
                }

                if (MapCursor.CenterCursorScreenCoordinates.Y < NorthBound)
                {
                    MoveCameraInDirection(CameraDirection.Up);
                }

                if (MapCursor.CenterCursorScreenCoordinates.Y > SouthBound)
                {
                    MoveCameraInDirection(CameraDirection.Down);
                }
            }

            if (targetPosition == currentPosition)
            {
                movingCameraToCursor = false;
            }
        }

        private float WestBound
        {
            get
            {
                return 0 + HorizontalCursorThreshold +
                       (GameContext.MapCursor.RenderSprite.Width * CurrentZoom);
            }
        }

        private float EastBound
        {
            get
            {
                return GameDriver.ScreenSize.X - HorizontalCursorThreshold -
                       (GameContext.MapCursor.RenderSprite.Width * CurrentZoom);
            }
        }

        private float NorthBound
        {
            get { return 0 + TopCursorThreshold; }
        }

        private float SouthBound
        {
            get
            {
                return GameDriver.ScreenSize.Y - BottomCursorThreshold -
                       (GameContext.MapCursor.RenderSprite.Height * CurrentZoom);
            }
        }

        private void CorrectCameraToMap()
        {
            CorrectPositionToMap(MapContainer.MapGridSize, currentPosition);
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
                    currentPosition.X = 0;
                    targetPosition.X = currentPosition.X;

                    //If new position would be beyond Right Edge, just center
                    if (CameraIsBeyondRightEdge(mapSize, currentPosition)) CenterHorizontally();
                }
                else if (CameraIsBeyondRightEdge(mapSize, position))
                {
                    currentPosition.X = RightEdge(mapSize) / CurrentZoom;
                    if (targetPosition.X < currentPosition.X)
                    {
                        targetPosition.X = currentPosition.X;
                    }

                    //If new position would be beyond Left Edge, just center
                    if (CameraIsBeyondLeftEdge(currentPosition)) CenterHorizontally();
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
                    currentPosition.Y = 0;
                    targetPosition.Y = currentPosition.Y;
                    //If new position would be beyond Bottom Edge, just center
                    if (CameraIsBeyondBottomEdge(mapSize, currentPosition)) CenterVertically();
                }
                else if (CameraIsBeyondBottomEdge(mapSize, position))
                {
                    currentPosition.Y = BottomEdge(mapSize) / CurrentZoom;
                    if (targetPosition.Y < currentPosition.Y)
                    {
                        targetPosition.Y = currentPosition.Y;
                    }

                    //If new position would be beyond Top Edge, just center
                    if (CameraIsBeyondTopEdge(currentPosition)) CenterVertically();
                }
            }
        }

        private void CenterVertically()
        {
            currentPosition.Y = (GameDriver.ScreenSize.Y - MapContainer.MapScreenSizeInPixels.Y) / 2;
            targetPosition.Y = currentPosition.Y;
        }

        private void CenterHorizontally()
        {
            currentPosition.X = (GameDriver.ScreenSize.X - MapContainer.MapScreenSizeInPixels.X) / 2;
            targetPosition.X = currentPosition.X;
        }

        private bool CameraIsBeyondBottomEdge(Vector2 mapSize, Vector2 position)
        {
            return position.Y * CurrentZoom < BottomEdge(mapSize);
        }

        private bool CameraIsBeyondRightEdge(Vector2 mapSize, Vector2 position)
        {
            return position.X * CurrentZoom < RightEdge(mapSize);
        }

        private bool CameraIsBeyondTopEdge(Vector2 position)
        {
            return position.Y > 0;
        }

        private bool CameraIsBeyondLeftEdge(Vector2 position)
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
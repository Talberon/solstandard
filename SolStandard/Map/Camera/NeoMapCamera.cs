using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Map.Camera
{
    public class NeoMapCamera : IMapCamera
    {
        private OrthographicCamera OrthographicCamera { get; }
        private CameraSmoother Smoother { get; }

        public float CurrentZoom => OrthographicCamera.Zoom;
        public float TargetZoom => Smoother.TargetZoom;
        public Vector2 CurrentPosition => OrthographicCamera.Position;
        public Vector2 TargetPosition => Smoother.TargetPosition;
        public Matrix CameraMatrix => OrthographicCamera.GetViewMatrix();

        private static readonly Dictionary<IMapCamera.ZoomLevel, float> ZoomLevels =
            new Dictionary<IMapCamera.ZoomLevel, float>
            {
                {IMapCamera.ZoomLevel.Far, FarZoom},
                {IMapCamera.ZoomLevel.Default, DefaultZoomLevel},
                {IMapCamera.ZoomLevel.Close, CloseZoom},
                {IMapCamera.ZoomLevel.Combat, CombatZoom}
            };

        private const double FloatTolerance = 0.01;

        private const float FarZoom = 1.4f;
        private const float DefaultZoomLevel = 2;
        private const float CloseZoom = 3;
        private const float CombatZoom = 4;

        private IMapCamera.ZoomLevel lastZoom;

        public NeoMapCamera(OrthographicCamera orthographicCamera, CameraSmoother smoother)
        {
            OrthographicCamera = orthographicCamera;
            Smoother = smoother;
            lastZoom = IMapCamera.ZoomLevel.Default;
        }

        public void RevertToPreviousZoomLevel()
        {
            SetZoomLevel(lastZoom);
        }

        public void SetZoomLevel(IMapCamera.ZoomLevel zoomLevel)
        {
            float newTargetZoom = ZoomLevels[zoomLevel];
            Smoother.ZoomTowards(newTargetZoom);

            if (Math.Abs(Smoother.TargetZoom - newTargetZoom) > FloatTolerance)
            {
                lastZoom = zoomLevel;
            }
        }

        public void ZoomIn()
        {
            if (CurrentZoom >= ZoomLevels[IMapCamera.ZoomLevel.Close]) return;

            if (CurrentZoom >= ZoomLevels[IMapCamera.ZoomLevel.Default]) SetZoomLevel(IMapCamera.ZoomLevel.Close);
            else if (CurrentZoom >= ZoomLevels[IMapCamera.ZoomLevel.Far]) SetZoomLevel(IMapCamera.ZoomLevel.Default);
        }

        public void ZoomOut()
        {
            if (CurrentZoom <= ZoomLevels[IMapCamera.ZoomLevel.Far]) return;

            if (CurrentZoom <= ZoomLevels[IMapCamera.ZoomLevel.Default]) SetZoomLevel(IMapCamera.ZoomLevel.Far);
            else if (CurrentZoom <= ZoomLevels[IMapCamera.ZoomLevel.Close]) SetZoomLevel(IMapCamera.ZoomLevel.Default);
            else if (CurrentZoom <= ZoomLevels[IMapCamera.ZoomLevel.Combat]) SetZoomLevel(IMapCamera.ZoomLevel.Close);
        }

        public void UpdateEveryFrame()
        {
            Smoother.Update();

            OrthographicCamera.LookAt(Smoother.CurrentPosition);
            OrthographicCamera.Zoom = Smoother.CurrentZoom;
        }

        public void SnapCameraCenterToCursor()
        {
            Smoother.SnapMoveTo(GlobalContext.MapCursor.CenterTargetPixelPoint);
        }

        public void CenterCameraToCursor()
        {
            Smoother.MoveTowards(GlobalContext.MapCursor.CenterTargetPixelPoint);
        }

        public void MoveCameraInDirection(CameraDirection direction, float panRateOverride)
        {
            Vector2 targetPosition = Smoother.TargetPosition;

            switch (direction)
            {
                case CameraDirection.Up:
                    targetPosition.Y -= panRateOverride;
                    break;
                case CameraDirection.Down:
                    targetPosition.Y += panRateOverride;
                    break;
                case CameraDirection.Left:
                    targetPosition.X -= panRateOverride;
                    break;
                case CameraDirection.Right:
                    targetPosition.X += panRateOverride;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            Smoother.MoveTowards(targetPosition);
        }

        public void StopMovingCamera()
        {
            Smoother.SnapMoveTo(Smoother.CurrentPosition);
        }

        public void StartMovingCameraToCursor()
        {
            CenterCameraToCursor();
        }
    }
}
using Microsoft.Xna.Framework;

namespace SolStandard.Map.Camera
{
    public interface IMapCamera
    {
        public enum ZoomLevel
        {
            Far,
            Default,
            Close,
            Combat
        }
        
        float CurrentZoom { get; }
        float TargetZoom { get; }
        Vector2 CurrentPosition { get; }
        Vector2 TargetPosition { get; }
        Matrix CameraMatrix { get; }
        void RevertToPreviousZoomLevel();
        void SetZoomLevel(ZoomLevel zoomLevel);
        void ZoomIn();
        void ZoomOut();
        void UpdateEveryFrame();
        void SnapCameraCenterToCursor();
        void CenterCameraToCursor();
        void MoveCameraInDirection(CameraDirection direction, float panRateOverride);
        void StopMovingCamera();
        void StartMovingCameraToCursor();
    }
}
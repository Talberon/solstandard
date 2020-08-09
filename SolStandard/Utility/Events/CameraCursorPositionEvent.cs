using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events
{
    public class CameraCursorPositionEvent : IEvent
    {
        private readonly Vector2 targetCameraPosition;
        public bool Complete { get; private set; }

        public CameraCursorPositionEvent(Vector2 targetCameraPosition)
        {
            this.targetCameraPosition = targetCameraPosition;
        }

        public void Continue()
        {
            GameContext.MapCursor.SnapCameraAndCursorToCoordinates(targetCameraPosition);

            if (GameContext.GameMapContext.MapContainer.MapCursor.IsOnScreen)
            {
                GameContext.GameMapContext.MapContainer.MapCamera.CenterCameraToCursor();
            }
            else
            {
                GameContext.GameMapContext.MapContainer.MapCamera.SnapCameraCenterToCursor();
            }

            Complete = true;
        }
    }
}
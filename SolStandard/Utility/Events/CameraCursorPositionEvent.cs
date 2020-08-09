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
            GlobalContext.MapCursor.SnapCameraAndCursorToCoordinates(targetCameraPosition);

            if (GlobalContext.GameMapContext.MapContainer.MapCursor.IsOnScreen)
            {
                GlobalContext.GameMapContext.MapContainer.MapCamera.CenterCameraToCursor();
            }
            else
            {
                GlobalContext.GameMapContext.MapContainer.MapCamera.SnapCameraCenterToCursor();
            }

            Complete = true;
        }
    }
}
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class ToastAtCoordinatesEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly string message;
        private readonly Vector2 toastCoordinates;
        private readonly int duration;

        public ToastAtCoordinatesEvent(Vector2 toastCoordinates, string message, int duration = 50)
        {
            this.message = message;
            this.toastCoordinates = toastCoordinates;
            this.duration = duration;
        }

        public void Continue()
        {
            AssetManager.MenuConfirmSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                message,
                toastCoordinates,
                duration
            );
            Complete = true;
        }
    }
}
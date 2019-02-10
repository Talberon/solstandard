using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class ToastAtCoordinatesEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly Vector2 toastCoordinates;
        private readonly string message;
        private readonly int duration;

        public ToastAtCoordinatesEvent(Vector2 toastCoordinates, string message, int duration = 50)
        {
            this.toastCoordinates = toastCoordinates;
            this.message = message;
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
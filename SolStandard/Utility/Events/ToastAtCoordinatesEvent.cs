using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Events
{
    public class ToastAtCoordinatesEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly string message;
        private readonly Vector2 toastCoordinates;
        private readonly int duration;
        private readonly ISoundEffect soundEffect;

        public ToastAtCoordinatesEvent(Vector2 toastCoordinates, string message, ISoundEffect soundEffect = null,
            int duration = 50)
        {
            this.message = message;
            this.soundEffect = soundEffect;
            this.toastCoordinates = toastCoordinates;
            this.duration = duration;
        }

        public ToastAtCoordinatesEvent(Vector2 toastCoordinates, string message, int duration) :
            this(toastCoordinates, message, null, duration)
        {
        }

        public void Continue()
        {
            soundEffect?.Play();

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                message,
                toastCoordinates,
                duration
            );
            Complete = true;
        }
    }
}
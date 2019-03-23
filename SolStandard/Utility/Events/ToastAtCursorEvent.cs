using SolStandard.Containers.Contexts;
using SolStandard.Utility.Monogame;

namespace SolStandard.Utility.Events
{
    public class ToastAtCursorEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly string message;
        private readonly int duration;
        private readonly ISoundEffect soundEffect;

        public ToastAtCursorEvent(string message, ISoundEffect soundEffect = null,
            int duration = 50)
        {
            this.message = message;
            this.soundEffect = soundEffect;
            this.duration = duration;
        }

        public ToastAtCursorEvent(string message, int duration) :
            this(message, null, duration)
        {
        }

        public void Continue()
        {
            if (soundEffect != null) soundEffect.Play();

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(message, duration);
            
            Complete = true;
        }
    }
}
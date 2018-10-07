using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class ToggleLockEvent : IEvent
    {
        private readonly ILockable lockable;
        public bool Complete { get; private set; }

        public ToggleLockEvent(ILockable lockable)
        {
            this.lockable = lockable;
        }

        public void Continue()
        {
            lockable.ToggleLock();
            //TODO Play ToggleLock SFX
            Complete = true;
        }
    }
}
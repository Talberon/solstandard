using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class TriggerEntityEvent : IEvent
    {
        private readonly IRemotelyTriggerable remotelyTriggerable;

        public TriggerEntityEvent(IRemotelyTriggerable remotelyTriggerable)
        {
            this.remotelyTriggerable = remotelyTriggerable;
        }
        
        public bool Complete { get; private set; }
        public void Continue()
        {
            remotelyTriggerable.RemoteTrigger();
            Complete = true;
        }
    }
}
using System;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class AdHocEvent : NetworkEvent
    {
        public delegate void EventAction();

        private readonly EventAction eventAction;

        public AdHocEvent(EventAction eventAction)
        {
            this.eventAction = eventAction;
        }

        public override void Continue()
        {
            eventAction.Invoke();
            Complete = true;
        }
    }
}
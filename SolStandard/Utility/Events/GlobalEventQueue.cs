using System.Collections.Generic;

namespace SolStandard.Utility.Events
{
    public static class GlobalEventQueue
    {
        /*
         * Enqueue several events to occur one after another
         * For example, play out a series of animations before starting combat.
         */

        private static readonly Queue<IEvent> EventSequence = new Queue<IEvent>();
        private static IEvent _currentEvent;

        private static bool AllActionsComplete
        {
            get { return EventSequence.Count == 0 && (_currentEvent == null || _currentEvent.Complete); }
        }

        public static void QueueEvents(Queue<IEvent> eventSequence)
        {
            foreach (IEvent queuedEvent in eventSequence)
            {
                EventSequence.Enqueue(queuedEvent);
            }

            if (EventSequence.Count > 0)
            {
                _currentEvent = EventSequence.Dequeue();
            }
        }

        public static bool UpdateEventsEveryFrame()
        {
            if (_currentEvent != null && !_currentEvent.Complete)
            {
                _currentEvent.Continue();
            }
            else if (EventSequence.Count > 0)
            {
                _currentEvent = EventSequence.Dequeue();
            }

            return AllActionsComplete;
        }
    }
}
using System.Collections.Generic;

namespace SolStandard.Utility.Events
{
    public static class GlobalEventQueue
    {
        /*
         * Enqueue several events to occur one after another
         * For example, play out a series of animations before starting combat.
         */

        private static Queue<IEvent> _eventSequence = new Queue<IEvent>();
        private static IEvent _currentEvent;

        public static bool AllActionsComplete
        {
            get { return _eventSequence.Count == 0 && (_currentEvent == null || _currentEvent.Complete); }
        }

        public static void QueueEvents(Queue<IEvent> eventSequence)
        {
            _eventSequence = eventSequence;
            _currentEvent = eventSequence.Dequeue();
        }

        public static bool UpdateEventsEveryFrame()
        {
            if (_currentEvent != null && !_currentEvent.Complete)
            {
                _currentEvent.Continue();
            }
            else if (_eventSequence.Count > 0)
            {
                _currentEvent = _eventSequence.Dequeue();
            }

            return AllActionsComplete;
        }
    }
}
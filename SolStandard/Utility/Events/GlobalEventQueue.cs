using System.Collections.Generic;
using SolStandard.Utility.Events.Network;

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
                QueueSingleEvent(queuedEvent);
            }
        }

        public static void QueueSingleEvent(IEvent eventToQueue)
        {
            NetworkEvent networkEvent = eventToQueue as NetworkEvent;
            if (networkEvent != null)
            {
                if (networkEvent.FromServer && GameDriver.ConnectedAsServer)
                {
                    //Send to client
                    GameDriver.ConnectionManager.SendEventMessageAsServer(networkEvent);
                }

                if (!networkEvent.FromServer && GameDriver.ConnectedAsClient)
                {
                    //Send to server
                    GameDriver.ConnectionManager.SendEventMessageAsClient(networkEvent);
                }
            }
            EventSequence.Enqueue(eventToQueue);
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
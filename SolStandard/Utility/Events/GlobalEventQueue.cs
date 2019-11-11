using System.Collections.Generic;
using NLog;
using SolStandard.Utility.Events.Network;

namespace SolStandard.Utility.Events
{
    public static class GlobalEventQueue
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Queue<IEvent> EventSequence = new Queue<IEvent>();
        private static IEvent _currentEvent;

        private static bool AllActionsComplete =>
            EventSequence.Count == 0 && (_currentEvent == null || _currentEvent.Complete);

        public static void QueueEvents(IEnumerable<IEvent> eventSequence)
        {
            foreach (IEvent queuedEvent in eventSequence)
            {
                QueueSingleEvent(queuedEvent);
            }
        }

        public static void QueueSingleEvent(IEvent eventToQueue)
        {
            if (eventToQueue is NetworkEvent networkEvent)
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

            Logger.Trace("Queuing new event: {}", eventToQueue);
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
                Logger.Trace("Popped event from queue: {}", _currentEvent);
            }

            return AllActionsComplete;
        }

        public static void ClearEventQueue()
        {
            _currentEvent = null;
            Logger.Trace("Clearing event queue.");
            EventSequence.Clear();
        }
    }
}
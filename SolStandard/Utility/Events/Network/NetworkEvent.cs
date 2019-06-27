using System;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public abstract class NetworkEvent : IEvent
    {
        public bool FromServer { get; }
        public bool Complete { get; protected set; }
        public abstract void Continue();

        protected NetworkEvent()
        {
            FromServer = GameDriver.ConnectedAsServer;
        }

    }
}
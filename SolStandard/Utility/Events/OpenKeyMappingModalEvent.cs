using System;

namespace SolStandard.Utility.Events
{
    public class OpenKeyMappingModalEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            //TODO Implement me
            throw new NotImplementedException();
            Complete = true;
        }
    }
}
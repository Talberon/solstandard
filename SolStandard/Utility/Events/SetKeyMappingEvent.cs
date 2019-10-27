namespace SolStandard.Utility.Events
{
    public class SetKeyMappingEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            //TODO Figure out how to call InputConfigOption.SetNewInput()
            Complete = true;
        }
    }
}
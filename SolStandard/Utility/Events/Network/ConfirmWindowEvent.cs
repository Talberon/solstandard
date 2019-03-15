namespace SolStandard.Utility.Events.Network
{
    public class ConfirmWindowEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            //TODO Take the current game state and confirm the window to move forward
            throw new System.NotImplementedException();
        }
    }
}
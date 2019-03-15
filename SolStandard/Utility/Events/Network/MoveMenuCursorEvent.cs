namespace SolStandard.Utility.Events.Network
{
    public class MoveMenuCursorEvent : IEvent
    {
        public bool Complete { get; private set; }
        public void Continue()
        {
            //TODO Take a menu argument and move the appropriate cursor
            throw new System.NotImplementedException();
        }
    }
}
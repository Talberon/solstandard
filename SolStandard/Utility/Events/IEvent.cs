namespace SolStandard.Utility.Events
{
    public interface IEvent
    {
        bool Complete { get; }
        void Continue();
    }
}
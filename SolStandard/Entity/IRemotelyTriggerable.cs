namespace SolStandard.Entity
{
    public interface IRemotelyTriggerable
    {
        void RemoteTrigger();
        bool CanTrigger { get; }
    }
}
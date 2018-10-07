namespace SolStandard.Entity
{
    public interface ILockable
    {
        bool IsLocked { get; }

        void ToggleLock();
    }
}
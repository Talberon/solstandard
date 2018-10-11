namespace SolStandard.Entity
{
    public interface IOpenable
    {
        bool IsOpen { get; }

        void Open();
        void Close();
    }
}
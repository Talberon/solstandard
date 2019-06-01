namespace SolStandard.Entity
{
    public interface IOpenable
    {
        bool IsOpen { get; }
        bool IsObstructed { get; }
        void Open();
        void Close();
    }
}
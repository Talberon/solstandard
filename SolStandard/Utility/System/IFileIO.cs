namespace SolStandard.Utility.System
{
    public interface IFileIO
    {
        void Save(string fileName, object content);
        T Load<T>(string fileName);
        bool FileExists(string fileName);
    }
}
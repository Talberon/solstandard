using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SolStandard.Utility.System
{
    public class TemporaryFilesIO : IFileIO
    {
        public const string GameFolder = "SolStandard";
        private static readonly string SaveFolder = Path.Combine(Path.GetTempPath(), GameFolder);

        public void Save(string fileName, object content)
        {
            Directory.CreateDirectory(SaveFolder);

            string fileToSaveTo = Path.Combine(SaveFolder, fileName);
            using Stream stream = File.OpenWrite(fileToSaveTo);
            new BinaryFormatter().Serialize(stream, content);
        }

        public T Load<T>(string fileName)
        {
            string fileToLoadFrom = Path.Combine(SaveFolder, fileName);

            if (!Directory.Exists(SaveFolder) || !File.Exists(fileToLoadFrom)) return default;

            using Stream stream = File.OpenRead(fileToLoadFrom);
            return (T) new BinaryFormatter().Deserialize(stream);
        }
        
        public bool FileExists(string fileName)
        {
            string fileToLoadFrom = Path.Combine(SaveFolder, fileName);
            return Directory.Exists(SaveFolder) && File.Exists(fileToLoadFrom);
        }
    }
}
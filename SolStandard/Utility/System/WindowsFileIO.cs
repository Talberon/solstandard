using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SolStandard.Utility.System
{
    public class WindowsFileIO : IFileIO
    {
        private const string GameFolder = "SolStandard";

        public void Save(string fileName, object content)
        {
            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), GameFolder));
            string fileToSaveTo = Path.Combine(Path.GetTempPath(), GameFolder, fileName);
            using (Stream stream = File.OpenWrite(fileToSaveTo))
            {
                new BinaryFormatter().Serialize(stream, content);
            }
        }

        public T Load<T>(string fileName)
        {
            string fileToLoadFrom = Path.Combine(Path.GetTempPath(), GameFolder, fileName);
            using (Stream stream = File.OpenRead(fileToLoadFrom))
            {
                return (T) new BinaryFormatter().Deserialize(stream);
            }
        }
    }
}
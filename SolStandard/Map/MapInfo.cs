namespace SolStandard.Map
{
    public class MapInfo
    {
        public string Title { get; private set; }
        public string FileName { get; private set; }


        public MapInfo(string title, string fileName)
        {
            Title = title;
            FileName = fileName;
        }
    }
}
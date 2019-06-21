namespace SolStandard.Map
{
    public class MapInfo
    {
        public string Title { get; }
        public string FileName { get; }


        public MapInfo(string title, string fileName)
        {
            Title = title;
            FileName = fileName;
        }
    }
}
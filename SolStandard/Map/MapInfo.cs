using SolStandard.Utility;

namespace SolStandard.Map
{
    public class MapInfo
    {
        public string Title { get; private set; }
        public string FileName { get; private set; }
        public SpriteAtlas PreviewImage { get; private set; }

        public MapInfo(string title, string fileName, SpriteAtlas previewImage)
        {
            Title = title;
            FileName = fileName;
            PreviewImage = previewImage;
        }
    }
}
namespace SolStandard.Containers
{
    public class GameContainer
    {
        private readonly MapLayer map;
        private readonly WindowLayer windows;

        public GameContainer(MapLayer map, WindowLayer windows)
        {
            this.map = map;
            this.windows = windows;
        }

        public MapLayer Map
        {
            get { return map; }
        }

        public WindowLayer Windows
        {
            get { return windows; }
        }
    }
}
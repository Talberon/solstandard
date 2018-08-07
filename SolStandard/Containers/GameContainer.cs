using System.Collections.Generic;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers
{
    public class GameContainer
    {
        private List<GameUnit> units;
        private readonly MapLayer map;
        private readonly WindowLayer windows;

        public GameContainer(MapLayer map, WindowLayer windows, List<GameUnit> units)
        {
            this.map = map;
            this.windows = windows;
            this.units = units;
        }

        public MapLayer Map
        {
            get { return map; }
        }

        public WindowLayer Windows
        {
            get { return windows; }
        }

        public List<GameUnit> Units
        {
            get { return units; }
        }
    }
}
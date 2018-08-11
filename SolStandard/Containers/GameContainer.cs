using System.Collections.Generic;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers
{
    public class GameContainer
    {
        private readonly List<GameUnit> units;
        private readonly MapLayer mapLayer;
        private readonly WindowLayer windowLayer;

        public GameContainer(MapLayer mapLayer, WindowLayer windowLayer, List<GameUnit> units)
        {
            this.mapLayer = mapLayer;
            this.windowLayer = windowLayer;
            this.units = units;
        }

        public MapLayer GetMapLayer()
        {
            return mapLayer;
        }

        public WindowLayer GetWindowLayer()
        {
            return windowLayer;
        }

        public List<GameUnit> GetUnits()
        {
            return units;
        }
    }
}
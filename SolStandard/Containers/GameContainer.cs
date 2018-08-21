using System.Collections.Generic;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers
{
    public class GameContainer
    {
        private readonly List<GameUnit> units;
        private readonly MapLayer mapLayer;
        private readonly MapUI mapUi;

        public GameContainer(MapLayer mapLayer, MapUI mapUi, List<GameUnit> units)
        {
            this.mapLayer = mapLayer;
            this.mapUi = mapUi;
            this.units = units;
        }

        public MapLayer MapLayer
        {
            get { return mapLayer; }
        }

        public MapUI MapUI
        {
            get { return mapUi; }
        }


        public List<GameUnit> Units
        {
            get { return units; }
        }
    }
}
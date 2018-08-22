using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers
{
    public class GameContainer
    {
        private readonly List<GameUnit> units;
        private readonly MapContext mapMapContext;
        private readonly MapUI mapUi;

        public GameContainer(MapContext mapMapContext, MapUI mapUi, List<GameUnit> units)
        {
            this.mapMapContext = mapMapContext;
            this.mapUi = mapUi;
            this.units = units;
        }


        public MapUI MapUI
        {
            get { return mapUi; }
        }


        public List<GameUnit> Units
        {
            get { return units; }
        }

        public MapContext MapContext
        {
            get { return mapMapContext; }
        }
    }
}
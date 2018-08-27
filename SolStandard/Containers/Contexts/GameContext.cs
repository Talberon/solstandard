using System.Collections.Generic;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers.Contexts
{
    public class GameContext
    {
        private readonly MapContext mapMapContext;
        private readonly BattleContext battleContext;
        private readonly MapUI mapUi;
        public static List<GameUnit> Units { get; private set; }

        public GameContext(MapContext mapMapContext, BattleContext battleContext, MapUI mapUi, List<GameUnit> units)
        {
            this.mapMapContext = mapMapContext;
            this.battleContext = battleContext;
            this.mapUi = mapUi;
            Units = units;
        }

        public MapUI MapUI
        {
            get { return mapUi; }
        }

        public MapContext MapContext
        {
            get { return mapMapContext; }
        }

        public BattleContext BattleContext
        {
            get { return battleContext; }
        }
    }
}
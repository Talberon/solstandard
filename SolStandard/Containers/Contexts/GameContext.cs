using System.Collections.Generic;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers.Contexts
{
    public class GameContext
    {
        private readonly MapContext mapContext;
        private readonly BattleContext battleContext;
        public static List<GameUnit> Units { get; private set; }

        public GameContext(MapContext mapContext, BattleContext battleContext, List<GameUnit> units)
        {
            this.mapContext = mapContext;
            this.battleContext = battleContext;
            Units = units;
        }

        public MapContext MapContext
        {
            get { return mapContext; }
        }

        public BattleContext BattleContext
        {
            get { return battleContext; }
        }
    }
}
using System.Collections.Generic;
using SolStandard.Containers.UI;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers.Contexts
{
    public class GameContext
    {
        private readonly MapContext mapMapContext;
        private readonly MapUI mapUi;
        private readonly CombatUI combatUi;
        public static List<GameUnit> Units { get; private set; }

        public GameContext(MapContext mapMapContext, MapUI mapUi, CombatUI combatUi, List<GameUnit> units)
        {
            this.mapMapContext = mapMapContext;
            this.mapUi = mapUi;
            this.combatUi = combatUi;
            Units = units;
        }

        public MapUI MapUI
        {
            get { return mapUi; }
        }

        public CombatUI CombatUi
        {
            get { return combatUi; }
        }

        public MapContext MapContext
        {
            get { return mapMapContext; }
        }
    }
}
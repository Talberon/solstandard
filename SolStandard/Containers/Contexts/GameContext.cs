using System.Collections.Generic;
using SolStandard.Entity.Unit;

namespace SolStandard.Containers.Contexts
{
    public class GameContext
    {
        private readonly MapContext mapContext;
        private readonly BattleContext battleContext;
        private static InitiativeContext _initiativeContext;

        public GameContext(MapContext mapContext, BattleContext battleContext, InitiativeContext initiativeContext)
        {
            this.mapContext = mapContext;
            this.battleContext = battleContext;
            _initiativeContext = initiativeContext;
        }

        public static List<GameUnit> Units
        {
            get { return _initiativeContext.InitiativeList; }
        }

        public static GameUnit ActiveUnit
        {
            get { return _initiativeContext.CurrentActiveUnit; }
        }

        public MapContext MapContext
        {
            get { return mapContext; }
        }

        public BattleContext BattleContext
        {
            get { return battleContext; }
        }

        public InitiativeContext InitiativeContext
        {
            get { return _initiativeContext; }
        }
    }
}
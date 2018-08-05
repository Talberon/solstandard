using SolStandard.Map.Objects;

namespace SolStandard.Entity.Unit
{
    public enum UnitClass
    {
        Champion,
        Archer,
        Mage
    }

    public enum Team
    {
        Red,
        Blue
    }
    
    public class GameUnit : GameEntity
    {
        private Team team;
        private UnitClass unitClass;
        
        private readonly UnitStatistics stats;
        //private List<Object> statusEffects; //TODO Add Buffs/Debuffs lists
        
        public GameUnit(string id, Team team, UnitClass unitClass, MapEntity mapEntity, UnitStatistics stats) : base(id, mapEntity)
        {
            this.team = team;
            this.unitClass = unitClass;
            this.stats = stats;
        }

        public UnitStatistics Stats
        {
            get { return stats; }
        }
    }
}
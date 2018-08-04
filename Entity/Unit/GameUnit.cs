using System;
using System.Collections.Generic;
using SolStandard.Map.Objects;
using SolStandard.Map.Objects.EntityProps;

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
        
        //TODO Implement Bonuses
        
        public GameUnit(string id, Team team, UnitClass unitClass, MapEntity mapEntity, List<EntityProp> properties, UnitStatistics stats) : base(id, mapEntity, properties)
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
namespace SolStandard.Map.Objects.Units
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
    
    public abstract class Unit
    {
        private UnitClass unitClass;
        private Team team;
    }
}
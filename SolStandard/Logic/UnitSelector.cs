using System.Collections.Generic;
using System.Linq;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements;

namespace SolStandard.Logic
{
    public static class UnitSelector
    {
        public static GameUnit SelectUnit(IEnumerable<GameUnit> units, MapEntity unit)
        {
            return units.FirstOrDefault(gameUnit => gameUnit.MapInfo == unit);
        }
    }
}
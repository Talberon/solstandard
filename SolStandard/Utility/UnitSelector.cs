using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements;

namespace SolStandard.Utility
{
    public static class UnitSelector
    {
        public static GameUnit SelectUnit(MapEntity unit)
        {
            if (unit == null) return null;
            
            return GameContext.Units.First(gameUnit => gameUnit.UnitEntity == unit);
        }

        public static MapEntity FindOtherUnitEntityAtCoordinates(Vector2 coordinates, MapEntity excludedEntity)
        {
            foreach (GameUnit unit in GameContext.Units)
            {
                if (unit.UnitEntity != null && unit.UnitEntity != excludedEntity)
                {
                    if (unit.UnitEntity.MapCoordinates == coordinates) return unit.UnitEntity;
                }
            }

            return null;
        }
    }
}
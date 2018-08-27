using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Map.Elements;

namespace SolStandard.Logic
{
    public class UnitSelector
    {
        public static GameUnit SelectUnit(MapEntity unit)
        {
            if (unit == null) return null;
            
            return GameContext.Units.First(gameUnit => gameUnit.MapEntity == unit);
        }

        public static MapEntity FindUnitEntityAtCoordinates(Vector2 coordinates)
        {
            foreach (GameUnit unit in GameContext.Units)
            {
                if (unit.MapEntity != null)
                {
                    if (unit.MapEntity.MapCoordinates == coordinates) return unit.MapEntity;
                }
            }

            return null;
        }
    }
}
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
            return GameContext.Units.FirstOrDefault(gameUnit => gameUnit.MapEntity == unit);
        }

        public static MapEntity FindUnitEntityAtCoordinates(Vector2 coordinates)
        {
            foreach (GameUnit unit in GameContext.Units)
            {
                if (unit.MapEntity.MapCoordinates == coordinates) return unit.MapEntity;
            }

            return null;
        }
    }
}
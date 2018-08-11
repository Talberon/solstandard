using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Map.Objects;

namespace SolStandard.Logic
{
    public static class UnitSelector
    {
        public static GameUnit SelectUnit(IEnumerable<GameUnit> units, MapEntity[,] unitGrid, Vector2 cursorCoordinates)
        {
            MapEntity unit = unitGrid[(int) cursorCoordinates.X, (int) cursorCoordinates.Y];

            return units.FirstOrDefault(gameUnit => gameUnit.MapInfo == unit);
        }
    }
}
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Skills;

namespace SolStandard.Map.Elements
{
    public interface IActionTile
    {
        int[] Range { get; }
        Vector2 MapCoordinates { get; }
        UnitSkill TileAction();
    }
}
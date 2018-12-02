using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Map.Elements
{
    public interface IActionTile
    {
        int[] InteractRange { get; }
        Vector2 MapCoordinates { get; }
        List<UnitAction> TileActions();
    }
}
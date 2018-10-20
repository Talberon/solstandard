﻿using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Map.Elements
{
    public interface IActionTile
    {
        int[] Range { get; }
        Vector2 MapCoordinates { get; }
        UnitAction TileAction();
    }
}
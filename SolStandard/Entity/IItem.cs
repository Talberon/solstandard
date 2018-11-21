﻿using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility;

namespace SolStandard.Entity
{
    public interface IItem
    {
        bool IsBroken { get; }
        IRenderable Icon { get; }
        string Name { get; }
        UnitAction UseAction();
        UnitAction DropAction();
    }
}
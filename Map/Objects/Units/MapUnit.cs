using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Map.Objects.EntityProps;
using SolStandard.Utility;

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

    public class MapUnit : MapEntity
    {
        private readonly UnitClass unitClass;
        private readonly Team team;

        public MapUnit(UnitClass unitClass, Team team, string name, IRenderable sprite, List<EntityProp> entityProps,
            Vector2 mapCoordinates) : base(name, sprite, entityProps, mapCoordinates)
        {
            this.unitClass = unitClass;
            this.team = team;
        }
    }
}
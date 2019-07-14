using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Crossing : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; }
        private readonly Direction crossDirection;

        public Crossing(string name, string type, IRenderable sprite, Vector2 mapCoordinates, Direction crossDirection)
            : base(name, type, sprite, mapCoordinates)
        {
            this.crossDirection = crossDirection;
            InteractRange = new[] {1};
        }


        public List<UnitAction> TileActions()
        {
            List<UnitAction> actions = new List<UnitAction>();

            if (UnitOnOppositeCrossDirection(crossDirection))
            {
                actions.Add(new CrossAction(MapCoordinates, crossDirection));
            }

            return actions;
        }

        private bool UnitOnOppositeCrossDirection(Direction directionToCross)
        {
            Vector2 unitCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;

            switch (directionToCross)
            {
                case Direction.None:
                    return UnitAction.SourceNorthOfTarget(unitCoordinates, MapCoordinates) ^
                           UnitAction.SourceEastOfTarget(unitCoordinates, MapCoordinates) ^
                           UnitAction.SourceSouthOfTarget(unitCoordinates, MapCoordinates) ^
                           UnitAction.SourceWestOfTarget(unitCoordinates, MapCoordinates);
                case Direction.Up:
                    return UnitAction.SourceSouthOfTarget(unitCoordinates, MapCoordinates);
                case Direction.Right:
                    return UnitAction.SourceWestOfTarget(unitCoordinates, MapCoordinates);
                case Direction.Down:
                    return UnitAction.SourceNorthOfTarget(unitCoordinates, MapCoordinates);
                case Direction.Left:
                    return UnitAction.SourceEastOfTarget(unitCoordinates, MapCoordinates);
                default:
                    throw new ArgumentOutOfRangeException(nameof(directionToCross), directionToCross, null);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
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
            CanMove = false;
        }


        public List<UnitAction> TileActions()
        {
            var actions = new List<UnitAction>();

            if (UnitOnOppositeCrossDirection(crossDirection))
            {
                actions.Add(new CrossAction(MapCoordinates, crossDirection));
            }

            return actions;
        }

        private bool UnitOnOppositeCrossDirection(Direction directionToCross)
        {
            Vector2 unitCoordinates = GlobalContext.ActiveUnit.UnitEntity.MapCoordinates;

            return directionToCross switch
            {
                Direction.None => (UnitAction.SourceNorthOfTarget(unitCoordinates, MapCoordinates) ^
                                   UnitAction.SourceEastOfTarget(unitCoordinates, MapCoordinates) ^
                                   UnitAction.SourceSouthOfTarget(unitCoordinates, MapCoordinates) ^
                                   UnitAction.SourceWestOfTarget(unitCoordinates, MapCoordinates)),
                Direction.Up => UnitAction.SourceSouthOfTarget(unitCoordinates, MapCoordinates),
                Direction.Right => UnitAction.SourceWestOfTarget(unitCoordinates, MapCoordinates),
                Direction.Down => UnitAction.SourceNorthOfTarget(unitCoordinates, MapCoordinates),
                Direction.Left => UnitAction.SourceEastOfTarget(unitCoordinates, MapCoordinates),
                _ => throw new ArgumentOutOfRangeException(nameof(directionToCross), directionToCross, null)
            };
        }
    }
}
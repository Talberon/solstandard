using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Cavalier
{
    public class CmdDoubleTime : UnitAction, ICommandAction
    {
        private readonly int cmdCost;
        private readonly int statModifier;
        private readonly int duration;

        public CmdDoubleTime(int cmdCost, int duration, int statModifier) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Double Time",
            description:
            $"Give all allies in range a buff to {UnitStatistics.Abbreviation[Stats.Mv]} by [+{statModifier}] for [{duration}] turns." +
            Environment.NewLine + $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1, 2, 3},
            freeAction: false
        )
        {
            this.cmdCost = cmdCost;
            this.statModifier = statModifier;
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!CanAffordCommandCost(GlobalContext.ActiveUnit, cmdCost))
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                GlobalContext.ActiveUnit.RemoveCommandPoints(cmdCost);
                BuffAlliesInRange();
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not an ally in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private void BuffAlliesInRange()
        {
            IEnumerable<MapSlice> slicesInRange = MapContainer.GetMapElementsFromLayer(Layer.Dynamic)
                .Select(tile => MapContainer.GetMapSliceAtCoordinates(tile.MapCoordinates));
            MapContainer.ClearDynamicAndPreviewGrids();

            var eventQueue = new Queue<IEvent>();

            foreach (MapSlice position in slicesInRange)
            {
                GameUnit unitAtPosition = UnitSelector.SelectUnit(position.UnitEntity);
                bool allyIsAtPosition = unitAtPosition != null &&
                                        unitAtPosition.Team == GlobalContext.ActiveTeam;
                if (allyIsAtPosition)
                {
                    eventQueue.Enqueue(
                        new CastStatusEffectEvent(unitAtPosition, new MoveStatModifier(duration, statModifier))
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(30));
                }
            }

            eventQueue.Enqueue(new EndTurnEvent());
            GlobalEventQueue.QueueEvents(eventQueue);
        }
    }
}
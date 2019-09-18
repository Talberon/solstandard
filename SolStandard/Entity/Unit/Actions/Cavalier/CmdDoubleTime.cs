using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
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

            if (!CanAffordCommandCost(GameContext.ActiveUnit, cmdCost))
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
            }

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                GameContext.ActiveUnit.RemoveCommandPoints(cmdCost);
                BuffAlliesInRange();
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not an ally in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private void BuffAlliesInRange()
        {
            IEnumerable<MapSlice> slicesInRange = MapContainer.GetMapElementsFromLayer(Layer.Dynamic)
                .Select(tile => MapContainer.GetMapSliceAtCoordinates(tile.MapCoordinates));
            MapContainer.ClearDynamicAndPreviewGrids();

            Queue<IEvent> eventQueue = new Queue<IEvent>();

            foreach (MapSlice position in slicesInRange)
            {
                GameUnit unitAtPosition = UnitSelector.SelectUnit(position.UnitEntity);
                bool allyIsAtPosition = unitAtPosition != null &&
                                        unitAtPosition.Team == GameContext.ActiveTeam;
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
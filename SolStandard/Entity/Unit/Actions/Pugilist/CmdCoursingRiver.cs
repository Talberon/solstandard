using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Pugilist
{
    public class CmdCoursingRiver : UnitAction
    {
        private readonly int cmdCost;
        private readonly int flowStacks;
        private readonly int stackDuration;

        public CmdCoursingRiver(int cmdCost, int flowStacks, int stackDuration) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Coursing River",
            description: $"Immediately gain [{flowStacks}] stacks of {FlowStrike.BuffName} as a free action. " +
                         Environment.NewLine +
                         $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.cmdCost = cmdCost;
            this.flowStacks = flowStacks;
            this.stackDuration = stackDuration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!CanAffordCommandCost(GameContext.ActiveUnit, cmdCost))
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                GameUnit activeUnit = GameContext.ActiveUnit;
                FlowStatus currentFlow =
                    activeUnit.StatusEffects.SingleOrDefault(status => status is FlowStatus) as FlowStatus;

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(
                        activeUnit,
                        new FlowStatus(
                            Icon,
                            stackDuration,
                            currentFlow?.FlowStacks + flowStacks ?? flowStacks,
                            FlowStrike.BuffName
                        )
                    )
                );
                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
                
                activeUnit.RemoveCommandPoints(cmdCost);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}
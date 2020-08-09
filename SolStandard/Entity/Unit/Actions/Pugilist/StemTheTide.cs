using System;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses.Pugilist;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Pugilist
{
    public class StemTheTide : UnitAction
    {
        private readonly int recoveryPerStack;
        private readonly int flowDuration;

        public StemTheTide(int recoveryPerStack, int flowDuration) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.StemTheTide, GameDriver.CellSizeVector),
            name: "Stem the Tide",
            description:
            $"Remove a stack of {FlowStrike.BuffName} and regenerate {recoveryPerStack} {UnitStatistics.Abbreviation[Stats.Armor]}." +
            Environment.NewLine + $"Resets {FlowStrike.BuffName} status duration to {flowDuration}T.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.recoveryPerStack = recoveryPerStack;
            this.flowDuration = flowDuration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                if (PugilistHasFlowStacks(targetUnit))
                {
                    if (UnitIsMissingArmor(targetUnit))
                    {
                        var statusToRemove =
                            (FlowStatus) targetUnit.StatusEffects.First(status => status is FlowStatus);
                        int currentFlowStacks = statusToRemove.FlowStacks;

                        GlobalEventQueue.QueueSingleEvent(new RemoveStatusEffectEvent(targetUnit, statusToRemove));

                        if (currentFlowStacks > 1)
                        {
                            GlobalEventQueue.QueueSingleEvent(
                                new CastStatusEffectEvent(
                                    targetUnit,
                                    new FlowStatus(
                                        FlowStrike.BuffIcon,
                                        flowDuration,
                                        currentFlowStacks - 1,
                                        FlowStrike.BuffName
                                    )
                                )
                            );
                        }

                        GlobalEventQueue.QueueSingleEvent(new RegenerateArmorEvent(targetUnit, recoveryPerStack));
                        GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(10));
                        GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
                    }
                    else
                    {
                        GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                            $"Target does not have {FlowStrike.BuffName} stacks!", 50);
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                        $"Target does not have {FlowStrike.BuffName} stacks!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool PugilistHasFlowStacks(GameUnit targetUnit)
        {
            return targetUnit.Role == Role.Pugilist && targetUnit.StatusEffects.Any(status => status is FlowStatus);
        }

        private static bool UnitIsMissingArmor(GameUnit targetUnit)
        {
            return targetUnit.Stats.CurrentArmor != targetUnit.Stats.MaxArmor;
        }
    }
}
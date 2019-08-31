using System.Linq;
using SolStandard.Containers.Contexts;
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
        private readonly int multiplier;

        public StemTheTide(int multiplier) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.StemTheTide, GameDriver.CellSizeVector),
            name: "Stem the Tide",
            description:
            $"Remove all stacks of {FlowStrike.BuffName} and regenerate {multiplier} {UnitStatistics.Abbreviation[Stats.Armor]} for every stack.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.multiplier = multiplier;
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
                        FlowStatus statusToRemove =
                            (FlowStatus) targetUnit.StatusEffects.First(status => status is FlowStatus);
                        int amountToRegenerate = statusToRemove.FlowStacks * multiplier;

                        GlobalEventQueue.QueueSingleEvent(new RemoveStatusEffectEvent(targetUnit, statusToRemove));
                        GlobalEventQueue.QueueSingleEvent(new RegenerateArmorEvent(targetUnit, amountToRegenerate));
                        GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(10));
                        GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
                    }
                    else
                    {
                        GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                            $"Target does not have {FlowStrike.BuffName} stacks!", 50);
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                        $"Target does not have {FlowStrike.BuffName} stacks!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
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
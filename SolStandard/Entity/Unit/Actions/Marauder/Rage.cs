using System;
using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses.Marauder;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Marauder
{
    public class Rage : UnitAction
    {
        private readonly int missingHpPerPoint;
        private readonly int duration;

        public Rage(int duration, int missingHpPerPoint) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Rage, GameDriver.CellSizeVector),
            name: "Rage",
            description: "Increase own " + UnitStatistics.Abbreviation[Stats.Atk] + " by 1 for every " +
                         missingHpPerPoint + " missing " + UnitStatistics.Abbreviation[Stats.Hp] + " for " +
                         duration + " turns." + Environment.NewLine +
                         "Reduces " + UnitStatistics.Abbreviation[Stats.Retribution] + " by the same amount.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.missingHpPerPoint = missingHpPerPoint;
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                if (UnitIsAtMaxHP(targetUnit))
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                        UnitStatistics.Abbreviation[Stats.Hp] + " is maxed out!",
                        50
                    );
                    AssetManager.WarningSFX.Play();
                }
                else
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    int halfMissingHP =
                        (int) Math.Floor((float) (targetUnit.Stats.MaxHP - targetUnit.Stats.CurrentHP) /
                                         missingHpPerPoint);

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(
                        new CastStatusEffectEvent(targetUnit, new EnragedStatus(duration, halfMissingHP)));
                    eventQueue.Enqueue(new WaitFramesEvent(50));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool UnitIsAtMaxHP(GameUnit targetUnit)
        {
            return targetUnit.Stats.CurrentHP == targetUnit.Stats.MaxHP;
        }
    }
}
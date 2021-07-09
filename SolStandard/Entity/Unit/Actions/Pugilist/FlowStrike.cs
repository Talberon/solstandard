using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Actions.Lancer;
using SolStandard.Entity.Unit.Statuses.Pugilist;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Pugilist
{
    public class FlowStrike : UnitAction
    {
        public const string BuffName = "Flow";
        public static readonly IRenderable BuffIcon =
            SkillIconProvider.GetSkillIcon(SkillIcon.FlowStrike, GameDriver.CellSizeVector);

        private readonly int percent;
        private readonly int buffDuration;

        public FlowStrike(int percent, int buffDuration) : base(
            icon: BuffIcon,
            name: "Flow Strike",
            description: "Grants a stack of " + BuffName + ", then attacks a unit for " + percent +
                         "% damage (rounded up)." + Environment.NewLine +
                         "Each stack of " + BuffName + " increases " + UnitStatistics.Abbreviation[Stats.Atk] +
                         " and " + UnitStatistics.Abbreviation[Stats.Retribution] + " by 1.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1},
            freeAction: false
        )
        {
            this.percent = percent;
            this.buffDuration = buffDuration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GameUnit attacker = GlobalContext.ActiveUnit;
                var currentFlow =
                    attacker.StatusEffects.SingleOrDefault(status => status is FlowStatus) as FlowStatus;

                int atkDamage = Execute.ApplyPercentageRoundedUp(attacker.Stats.Atk, percent);
                var flowStrikeFist =
                    new WeaponStatistics(atkDamage, attacker.Stats.LuckModifier, attacker.Stats.CurrentAtkRange, 1);

                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(
                        attacker,
                        new FlowStatus(
                            Icon,
                            buffDuration,
                            currentFlow?.FlowStacks + 1 ?? 1,
                            BuffName
                        )
                    )
                );
                eventQueue.Enqueue(new WaitFramesEvent(30));
                eventQueue.Enqueue(new StartCombatEvent(
                    targetUnit, false, attacker.Stats.ApplyWeaponStatistics(flowStrikeFist, true)
                ));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}
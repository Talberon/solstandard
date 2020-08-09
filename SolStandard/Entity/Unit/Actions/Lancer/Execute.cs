using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Statuses.Lancer;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class Execute : UnitAction
    {
        private readonly int damagePercent;
        private readonly int buffDuration;
        private readonly int atkModifier;

        public Execute(int damagePercent, int buffDuration, int atkModifier) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Assassinate, GameDriver.CellSizeVector),
            name: "Execute",
            description: $"Attack a unit for {damagePercent}% damage (rounded up)." + Environment.NewLine +
                         $"If target is defeated, regenerate all {UnitStatistics.Abbreviation[Stats.Armor]} and gain a [{atkModifier} {UnitStatistics.Abbreviation[Stats.Atk]}] Up buff for {buffDuration} turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
            this.damagePercent = damagePercent;
            this.buffDuration = buffDuration;
            this.atkModifier = atkModifier;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Range = GlobalContext.ActiveUnit.Stats.CurrentAtkRange;
            base.GenerateActionGrid(origin, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit attacker = GlobalContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            int atkDamage = ApplyPercentageRoundedUp(attacker.Stats.Atk, damagePercent);
            var executionersKnife =
                new WeaponStatistics(atkDamage, 0, attacker.Stats.CurrentAtkRange, 1);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(attacker, new ExecutionerStatus(Icon, 0, buffDuration, atkModifier))
                );
                eventQueue.Enqueue(
                    new StartCombatEvent(
                        targetUnit,
                        false,
                        attacker.Stats.ApplyWeaponStatistics(executionersKnife, true)
                    )
                );
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static int ApplyPercentageRoundedUp(int baseNumber, int percentageOfBaseNumber)
        {
            float remainingPercentage = 100 - percentageOfBaseNumber;
            int remainderToRemove = (int) Math.Floor(baseNumber * (remainingPercentage / 100));
            return baseNumber - remainderToRemove;
        }
    }
}
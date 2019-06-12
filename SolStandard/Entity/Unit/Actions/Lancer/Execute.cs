using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Statuses;
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
        private readonly int percent;

        public Execute(int percent) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Execute, new Vector2(GameDriver.CellSize)),
            name: "Execute",
            description: "Attack a unit for " + percent + "% damage (rounded up)." + Environment.NewLine +
                         "If target is defeated, regenerate all " +
                         UnitStatistics.Abbreviation[Stats.Armor] + " and gain an " +
                         UnitStatistics.Abbreviation[Stats.Atk] + " Up buff.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: null,
            freeAction: false
        )
        {
            this.percent = percent;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Range = GameContext.ActiveUnit.Stats.CurrentAtkRange;
            base.GenerateActionGrid(origin, mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit attacker = GameContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            int atkDamage = DamageValueRoundedUp(attacker.Stats.Atk, percent);
            WeaponStatistics executionersKnife =
                new WeaponStatistics(atkDamage, 0, attacker.Stats.CurrentAtkRange, 1);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(attacker, new ExecutionerStatus(Icon, 0)));
                eventQueue.Enqueue(
                    new StartCombatEvent(
                        targetUnit,
                        attacker.Stats.ApplyWeaponStatistics(executionersKnife)
                    )
                );
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static int DamageValueRoundedUp(int atk, int percentageDamage)
        {
            float remainingPercentage = 100 - percentageDamage;
            int damageToRemove = (int) Math.Floor(atk * (remainingPercentage / 100));
            return atk - damageToRemove;
        }
    }
}
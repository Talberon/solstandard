using System;
using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class BetwixtPlate : UnitAction
    {
        private readonly float percent;

        public BetwixtPlate(float percent) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BetwixtPlate, GameDriver.CellSizeVector),
            name: "Betwixt Plate",
            description: "Attack a unit for " + percent +
                         "% damage (rounded up) and ignore target's " +
                         UnitStatistics.Abbreviation[Stats.Armor] + ".",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1},
            freeAction: false
        )
        {
            this.percent = percent;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GameUnit attacker = GlobalContext.ActiveUnit;

                //Subtract the remaining percent damage from Attacker's ATK stat
                float remainingPercentage = 100 - percent;
                int damageModifier = (int) Math.Ceiling(attacker.Stats.Atk * (remainingPercentage / 100));

                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(
                        attacker,
                        new IgnoreArmorCombatStatus(Icon, 0, -damageModifier)
                    )
                );
                eventQueue.Enqueue(new StartCombatEvent(targetUnit));
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
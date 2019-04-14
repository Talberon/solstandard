using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Pugilist
{
    public class PressurePoint : UnitAction
    {
        private readonly float percent;

        public PressurePoint(float percent) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.PressurePoint, new Vector2(GameDriver.CellSize)),
            name: "Pressure Point",
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
                GameUnit attacker = GameContext.ActiveUnit;

                //Subtract the remaining percent damage from Attacker's ATK stat
                float remainingPercentage = 100 - percent;
                int damageModifier = (int) Math.Ceiling(attacker.Stats.Atk * (remainingPercentage / 100));

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
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
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}
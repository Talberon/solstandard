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
        public PressurePoint() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.PressurePoint, new Vector2(GameDriver.CellSize)),
            name: "Pressure Point",
            description: "Attack a unit for half damage (rounded down) and ignore target's " +
                         UnitStatistics.Abbreviation[Stats.Armor] + ".",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1}
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GameUnit attacker = GameContext.ActiveUnit;

                //The rounded-up value will be subtracted from the attacker's ATK stat to result in half rounded-down damage.
                int halfDamageRoundedUp = (int) Math.Ceiling((float) attacker.Stats.Atk / 2);
                AtkStatModifier halfDamageStatus = new AtkStatModifier(0, -halfDamageRoundedUp);

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(attacker, halfDamageStatus));
                eventQueue.Enqueue(new CastStatusEffectEvent(attacker, new IgnoreArmorCombatStatus(Icon, 0)));
                eventQueue.Enqueue(new StartCombatEvent(targetUnit));
                eventQueue.Enqueue(new RemoveStatusEffectEvent(attacker, halfDamageStatus));
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
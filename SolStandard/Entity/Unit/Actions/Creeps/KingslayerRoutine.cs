using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class KingslayerRoutine : BasicAttackRoutine
    {
        public KingslayerRoutine(bool independent) : base(
            independent,
            "Kingslayer Routine",
            "Prefers to attack enemy Commanders in range. Will default to any enemy if no commanders are near.",
            SkillIcon.Kingslayer
        )
        {
        }

        public override bool CanBeReadied(CreepUnit creepUnit)
        {
            return TilesWithinThreatRangeForUnit(creepUnit, Independent).Count > 0;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit attacker = GlobalContext.ActiveUnit;

            List<KeyValuePair<GameUnit, Vector2>> commandersInRange =
                CommandersInRange(TilesWithinThreatRangeForUnit(attacker, Independent));

            GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(30));
            if (commandersInRange.Count > 0)
            {
                PathToTargetAndAttack(commandersInRange, attacker);
            }
            else
            {
                GlobalEventQueue.QueueSingleEvent(
                    new ToastAtCursorEvent(
                        "No Commanders in range! " + Environment.NewLine + "Targeting any enemies in range.", 50
                    )
                );
                GlobalEventQueue.QueueSingleEvent(new SkippableWaitFramesEvent(50));
                base.ExecuteAction(targetSlice);
            }
        }

        private static List<KeyValuePair<GameUnit, Vector2>> CommandersInRange(
            IReadOnlyCollection<KeyValuePair<GameUnit, Vector2>> targets
        )
        {
            List<GameUnit> commanders = targets.Select(kvp => kvp.Key).Where(unit => unit.IsCommander).ToList();
            return targets.Where(kvp => commanders.Contains(kvp.Key)).ToList();
        }
    }
}
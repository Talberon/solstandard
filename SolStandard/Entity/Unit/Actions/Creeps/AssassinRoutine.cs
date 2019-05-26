using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Creeps
{
    public class AssassinRoutine : BasicAttackRoutine
    {
        public AssassinRoutine(bool independent) : base(
            independent,
            "Assassinate Routine",
            "Attacks a random enemy in range with the lowest " + UnitStatistics.Abbreviation[Stats.Hp] + " and " +
            UnitStatistics.Abbreviation[Stats.Armor] + ".",
            SkillIcon.Assassin
        )
        {
        }

        public override bool CanBeReadied(CreepUnit creepUnit)
        {
            return TilesWithinThreatRangeForUnit(creepUnit, Independent).Count > 0;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit attacker = GameContext.ActiveUnit;

            List<KeyValuePair<GameUnit, Vector2>> weakestTargetsInRange =
                TargetsWithLowestEffectiveHealth(TilesWithinThreatRangeForUnit(attacker, Independent));

            GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
            if (weakestTargetsInRange.Count > 0)
            {
                PathToTargetAndAttack(weakestTargetsInRange, attacker);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates("No valid targets in range!",
                    targetSlice.MapCoordinates, 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static List<KeyValuePair<GameUnit, Vector2>> TargetsWithLowestEffectiveHealth(
            IReadOnlyCollection<KeyValuePair<GameUnit, Vector2>> targets
        )
        {
            List<GameUnit> targetUnits = targets.Select(kvp => kvp.Key).ToList();
            int lowestEffectiveHealth = targetUnits.Min(unit => unit.Stats.CurrentHP + unit.Stats.CurrentArmor);
            return targets.Where(kvp => (kvp.Key.Stats.CurrentHP + kvp.Key.Stats.CurrentArmor) == lowestEffectiveHealth)
                .ToList();
        }
    }
}
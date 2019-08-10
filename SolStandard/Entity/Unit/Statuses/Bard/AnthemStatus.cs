using System.Linq;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.Combat;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Bard
{
    public class AnthemStatus : SongStatus
    {
        public AnthemStatus(int auraBonus, int selfBonus, int[] auraRange) : base(
            statusIcon: SkillIconProvider.GetSkillIcon(SkillIcon.AtkBuff, GameDriver.CellSizeVector),
            name:
            $"Anthem <+{UnitStatistics.Abbreviation[Stats.Atk]}/{UnitStatistics.Abbreviation[Stats.Retribution]} {auraBonus} Aura/{selfBonus} Solo>",
            description:
            $"Increases {UnitStatistics.Abbreviation[Stats.Atk]} and {UnitStatistics.Abbreviation[Stats.Retribution]} by [{auraBonus} Aura/{selfBonus} Solo] for units within the aura.",
            turnDuration: 99,
            new BonusStatistics(auraBonus, auraBonus, 0, 0),
            new BonusStatistics(selfBonus, selfBonus, 0, 0),
            auraRange,
            false
        )
        {
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
            base.ApplyEffect(target);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override bool UnitIsAffectedBySong(GameUnit unitAffected)
        {
            GameUnit singer = GameContext.Units.FirstOrDefault(unit => unit.StatusEffects.Contains(this));
            return singer != null &&
                   (unitAffected.Team == singer.Team && UnitIsAffectedBySong(unitAffected, this));
        }
    }
}
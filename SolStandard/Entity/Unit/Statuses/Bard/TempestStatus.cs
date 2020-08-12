using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Bard
{
    public class TempestStatus : SongStatus
    {
        public TempestStatus(int auraBonus, int selfBonus, int[] auraRange) : base(
            statusIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Tempest, GameDriver.CellSizeVector),
            name:
            $"Tempest <+{UnitStatistics.Abbreviation[Stats.Retribution]} {auraBonus} Aura/{selfBonus} Solo>",
            description:
            $"Increases {UnitStatistics.Abbreviation[Stats.Retribution]} by [{auraBonus} Aura/{selfBonus} Solo] for units within the aura.",
            turnDuration: 99,
            new BonusStatistics(0, auraBonus, 0, 0),
            new BonusStatistics(0, selfBonus, 0, 0),
            auraRange,
            false
        )
        {
            SongSprite = AnimatedSpriteProvider.GetAnimatedSprite(
                AnimationType.SongRetribution,
                GameDriver.CellSizeVector,
                SongAnimationFrameDelay,
                GetSongColor(GlobalContext.ActiveTeam)
            );
        }

        public override void ApplyEffect(GameUnit target)
        {
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
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
            GameUnit singer = GlobalContext.Units.FirstOrDefault(unit => unit.StatusEffects.Contains(this));
            return singer != null &&
                   (unitAffected.Team == singer.Team && UnitIsAffectedBySong(unitAffected, this));
        }
    }
}
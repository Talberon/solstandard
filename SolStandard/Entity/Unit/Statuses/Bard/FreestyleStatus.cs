using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Bard
{
    public class FreestyleStatus : SongStatus
    {
        public FreestyleStatus(int auraBonus, int selfBonus, int[] auraRange) : base(
            statusIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Freestyle, GameDriver.CellSizeVector),
            name: $"Freestyle <+{UnitStatistics.Abbreviation[Stats.Luck]} {auraBonus} Aura/{selfBonus} Solo>",
            description:
            $"Increases {UnitStatistics.Abbreviation[Stats.Luck]} by [{auraBonus} Aura/{selfBonus} Solo] for units within the aura.",
            turnDuration: 99,
            new BonusStatistics(0, 0, 0, auraBonus),
            new BonusStatistics(0, 0, 0, selfBonus),
            auraRange,
            false
        )
        {
            SongSprite = AnimatedSpriteProvider.GetAnimatedSprite(
                AnimationType.SongLuck,
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
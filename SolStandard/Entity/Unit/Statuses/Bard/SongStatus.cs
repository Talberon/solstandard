using System.Linq;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.Combat;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Statuses.Bard
{
    public abstract class SongStatus : StatusEffect
    {
        protected bool IsAuraEffect { get; set; }
        protected bool IsSelfEffect => !IsAuraEffect;
        private BonusStatistics AuraBonus { get; }
        private BonusStatistics SelfBonus { get; }
        private readonly int[] auraRange;

        public int[] AuraRange => IsAuraEffect ? auraRange : new[] {0};

        protected SongStatus(IRenderable statusIcon, string name, string description, int turnDuration,
            BonusStatistics auraBonus, BonusStatistics selfBonus, int[] auraRange, bool hasNotification)
            : base(statusIcon, name, description, turnDuration, hasNotification, false)
        {
            AuraBonus = auraBonus;
            SelfBonus = selfBonus;
            this.auraRange = auraRange;
        }

        public BonusStatistics ActiveBonus => IsAuraEffect ? AuraBonus : SelfBonus;

        public void SetToSelfEffect()
        {
            IsAuraEffect = false;
        }

        public void SetToAuraEffect()
        {
            IsAuraEffect = true;
        }

        public abstract bool UnitIsAffectedBySong(GameUnit unitAffected);

        public override void ApplyEffect(GameUnit target)
        {
            StatusEffect soloStatus = target.StatusEffects.FirstOrDefault(status => status is SoloStatus);
            soloStatus?.ApplyEffect(target);
            
            StatusEffect concertoStatus = target.StatusEffects.FirstOrDefault(status => status is ConcertoStatus);
            concertoStatus?.ApplyEffect(target);
        }

        protected static bool UnitIsAffectedBySong(GameUnit unitAffected, SongStatus song)
        {
            //TODO Differentiate between ally and enemies so bonuses aren't applied to enemies unless the skill specifically wants to

            GameUnit singer = GameContext.Units.FirstOrDefault(unit => unit.StatusEffects.Contains(song));

            if (singer == null || unitAffected == null || !singer.IsAlive || !unitAffected.IsAlive) return false;

            return RangeComparison.TargetIsWithinRangeOfOrigin(singer.UnitEntity.MapCoordinates, song.AuraRange,
                unitAffected.UnitEntity.MapCoordinates);
        }
    }
}
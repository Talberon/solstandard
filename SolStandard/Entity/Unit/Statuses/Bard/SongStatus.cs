using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Bard
{
    public abstract class SongStatus : StatusEffect
    {
        protected const int SongAnimationFrameDelay = 120;
        protected bool IsAuraEffect { get; set; }
        protected bool IsSelfEffect => !IsAuraEffect;
        private BonusStatistics AuraBonus { get; }
        private BonusStatistics SelfBonus { get; }
        private readonly int[] auraRange;
        public AnimatedSpriteSheet SongSprite { get; protected set; }

        public int[] AuraRange => IsAuraEffect ? auraRange : new[] {0};

        protected SongStatus(IRenderable statusIcon, string name, string description, int turnDuration,
            BonusStatistics auraBonus, BonusStatistics selfBonus, int[] auraRange, bool hasNotification)
            : base(statusIcon, name, description, turnDuration, hasNotification, false)
        {
            AuraBonus = auraBonus;
            SelfBonus = selfBonus;
            this.auraRange = auraRange;
            SongSprite = AnimatedSpriteProvider.GetAnimatedSprite(AnimationType.SongHymn, GameDriver.CellSizeVector,
                SongAnimationFrameDelay, GetSongColor(GlobalContext.ActiveTeam));
        }

        protected static Color GetSongColor(Team team)
        {
            return team switch
            {
                Team.Blue => new Color(80, 80, 255, 50),
                Team.Red => new Color(255, 70, 80, 50),
                _ => new Color(35, 100, 35, 100)
            };
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
            GameUnit singer = GlobalContext.Units.FirstOrDefault(unit => unit.StatusEffects.Contains(song));

            if (singer == null || unitAffected == null || !singer.IsAlive || !unitAffected.IsAlive) return false;

            return RangeComparison.TargetIsWithinRangeOfOrigin(singer.UnitEntity.MapCoordinates, song.AuraRange,
                unitAffected.UnitEntity.MapCoordinates);
        }
    }
}
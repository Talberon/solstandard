using System.Linq;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.Combat;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Statuses.Bard
{
    public class TempestStatus : SongStatus
    {
        private readonly int auraBonus;
        private readonly int selfBonus;

        public TempestStatus(int auraBonus, int selfBonus, int[] auraRange) : base(
            statusIcon: SkillIconProvider.GetSkillIcon(SkillIcon.DoubleTime, GameDriver.CellSizeVector),
            name: $"Tempest <+{UnitStatistics.Abbreviation[Stats.Mv]} {auraBonus} Aura/{selfBonus} Solo>",
            description:
            $"Applies a {UnitStatistics.Abbreviation[Stats.Mv]} buff [{auraBonus} Aura/{selfBonus} Solo] for units within the aura at the start of the turn.",
            turnDuration: 99,
            new BonusStatistics(0, 0, 0, 0),
            new BonusStatistics(0, 0, 0, 0),
            auraRange,
            true
        )
        {
            this.auraBonus = auraBonus;
            this.selfBonus = selfBonus;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
            base.ApplyEffect(target);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            GameUnit singer = GameContext.Units.FirstOrDefault(unit => unit.StatusEffects.Contains(this));

            if (singer == null || !singer.IsAlive) return;

            if (IsSelfEffect)
            {
                GlobalEventQueue.QueueSingleEvent(
                    new CastStatusEffectEvent(singer, new MoveStatModifier(1, selfBonus))
                );
                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
                return;
            }

            foreach (GameUnit ally in GameContext.Units.Where(unit => unit.Team == singer.Team))
            {
                if (UnitIsAffectedBySong(ally, this))
                {
                    GlobalEventQueue.QueueSingleEvent(
                        new CastStatusEffectEvent(ally, new MoveStatModifier(1, auraBonus))
                    );
                    GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
                }
            }
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
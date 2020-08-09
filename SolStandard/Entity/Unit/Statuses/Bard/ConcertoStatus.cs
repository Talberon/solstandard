using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Actions.Bard;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Bard
{
    public class ConcertoStatus : StatusEffect
    {
        public ConcertoStatus() : base(
            statusIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Concerto, GameDriver.CellSizeVector),
            name: "Playing: " + ModeConcerto.GroupSkillName,
            description: "Applies song effects to allies in range with reduced potency.",
            turnDuration: 99,
            hasNotification: false,
            canCleanse: false
        )
        {
        }

        public override void ApplyEffect(GameUnit target)
        {
            GameUnit singer = GlobalContext.Units.FirstOrDefault(unit => unit.StatusEffects.Contains(this));
            if (singer == null || !singer.IsAlive) return;

            List<SongStatus> songs =
                singer.StatusEffects.Where(status => status is SongStatus).Cast<SongStatus>().ToList();

            songs.ForEach(song => song.SetToAuraEffect());

            AssetManager.SkillBuffSFX.Play();
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing.
        }
    }
}
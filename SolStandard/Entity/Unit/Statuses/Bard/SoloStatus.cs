using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Actions.Bard;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Bard
{
    public class SoloStatus : StatusEffect
    {
        public SoloStatus() : base(
            //TODO New Icon
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Atk, GameDriver.CellSizeVector),
            name: ModeSolo.SoloSkillName,
            description: "Applies aura effects to self at increased potency.",
            turnDuration: 99,
            hasNotification: false,
            canCleanse: false
        )
        {
        }

        public override void ApplyEffect(GameUnit target)
        {
            GameUnit singer = GameContext.Units.FirstOrDefault(unit => unit.StatusEffects.Contains(this));
            if (singer == null || !singer.IsAlive) return;

            List<SongStatus> songs = singer.StatusEffects.Where(status => status is SongStatus).Cast<SongStatus>().ToList();
            songs.ForEach(song => song.SetToSelfEffect());
            
            AssetManager.SkillBuffSFX.Play();
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
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
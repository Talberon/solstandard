using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Marauder
{
    public class EnragedStatus : StatusEffect
    {
        private readonly int atkModifier;

        public EnragedStatus(int turnDuration, int atkModifier) :
            base(
                statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Atk, GameDriver.CellSizeVector),
                name: "Enraged! <+" + atkModifier + " " + UnitStatistics.Abbreviation[Stats.Atk] + " / " +
                      "-" + atkModifier + " " + UnitStatistics.Abbreviation[Stats.Retribution] + ">",
                description: "Increased attack power / Reduced retribution",
                turnDuration: turnDuration,
                hasNotification: false,
                canCleanse: true
            )
        {
            this.atkModifier = atkModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.AtkModifier += atkModifier;
            target.Stats.RetModifier -= atkModifier;

            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.AtkModifier -= atkModifier;
            target.Stats.RetModifier += atkModifier;
        }
    }
}
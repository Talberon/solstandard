using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class ImmobilizedStatus : StatusEffect
    {
        private const int FrameDelay = 15;
        private int mvModifier;

        public ImmobilizedStatus(int turnDuration) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.MvUp,
                GameDriver.CellSizeVector),
            name: "Immobilized!",
            description: "Unit cannot move.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: true
        )
        {
        }

        public override void ApplyEffect(GameUnit target)
        {
            mvModifier = target.Stats.BaseMv;
            target.Stats.MvModifier -= mvModifier;

            target.UnitEntity.UnitSpriteSheet.SetFrameDelay(FrameDelay);

            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
            AssetManager.SkillBuffSFX.Play();
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.MvModifier += mvModifier;
            target.UnitEntity?.UnitSpriteSheet.ResetFrameDelay();
        }
    }
}
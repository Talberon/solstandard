using SolStandard.Containers.Components.Global;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class ImmovableStatus : StatusEffect
    {
        public ImmovableStatus(IRenderable icon, int turnDuration) : base(
            statusIcon: icon,
            name: "Immovable!",
            description: "Cannot be moved by other unit abilities.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
            target.IsMovable = false;
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.IsMovable = true;
        }
    }
}
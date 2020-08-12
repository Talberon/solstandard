using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class ArmorRegeneration : StatusEffect
    {
        private readonly int armorModifier;

        public ArmorRegeneration(int turnDuration, int armorModifier) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.DefUp,
                GameDriver.CellSizeVector),
            name: UnitStatistics.Abbreviation[Stats.Armor] + " Regen! <+" + armorModifier + "/turn>",
            description: "Increased defensive power.",
            turnDuration: turnDuration,
            hasNotification: true,
            canCleanse: false
        )
        {
            this.armorModifier = armorModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            target.RecoverArmor(armorModifier);

            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                target.Id + " regenerates [" + armorModifier + "] " + UnitStatistics.Abbreviation[Stats.Armor] + "!",
                50
            );

            AssetManager.SkillBuffSFX.Play();
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }
    }
}
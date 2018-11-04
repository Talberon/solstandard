using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class RegenerateArmorEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly int armorPoints;

        public RegenerateArmorEvent(GameUnit targetUnit, int armorPoints)
        {
            this.targetUnit = targetUnit;
            this.armorPoints = armorPoints;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            AssetManager.SkillBuffSFX.Play();
            targetUnit.RecoverArmor(armorPoints);
            Complete = true;
        }
    }
}
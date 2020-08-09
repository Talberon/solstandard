using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Mage
{
    public class AlchemicalArmorStatus : StatusEffect
    {
        private readonly int blkModifier;
        private readonly int hpPerTurn;
        private readonly int amrPerTurn;

        public AlchemicalArmorStatus(int blkModifier, int hpPerTurn, int amrPerTurn, int turnDuration) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Block, GameDriver.CellSizeVector),
            name:
            $"Alchemical Armor [+{blkModifier} {UnitStatistics.Abbreviation[Stats.Block]}/+{hpPerTurn} {UnitStatistics.Abbreviation[Stats.Hp]}/+{amrPerTurn} {UnitStatistics.Abbreviation[Stats.Armor]}]",
            description: $"{UnitStatistics.Abbreviation[Stats.Block]} Up! " + Environment.NewLine +
                         $"+{hpPerTurn} {UnitStatistics.Abbreviation[Stats.Hp]} per turn. " + Environment.NewLine +
                         $"+{amrPerTurn} {UnitStatistics.Abbreviation[Stats.Armor]} per turn.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            this.blkModifier = blkModifier;
            this.hpPerTurn = hpPerTurn;
            this.amrPerTurn = amrPerTurn;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.BlkModifier += blkModifier;
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            target.RecoverArmor(amrPerTurn);
            target.RecoverHP(hpPerTurn);

            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                $"{target.Id} regenerates {amrPerTurn} {UnitStatistics.Abbreviation[Stats.Armor]} and {hpPerTurn} {UnitStatistics.Abbreviation[Stats.Hp]}!",
                50
            );

            AssetManager.SkillBuffSFX.Play();
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.BlkModifier -= blkModifier;
        }
    }
}
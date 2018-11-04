﻿using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class ArmorRegenerationUp : StatusEffect
    {
        private readonly int armorModifier;

        public ArmorRegenerationUp(int turnDuration, int armorModifier) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.DefUp,
                new Vector2(GameDriver.CellSize)),
            name: UnitStatistics.Abbreviation[Stats.Armor] + " Regen Up!",
            description: "Increased defensive power.",
            turnDuration: turnDuration
        )
        {
            this.armorModifier = armorModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            target.RecoverArmor(armorModifier);

            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                target.Id + " regenerates [" + armorModifier + "] extra " + UnitStatistics.Abbreviation[Stats.Armor] + "!",
                50
            );

            AssetManager.SkillBuffSFX.Play();
        }

        protected override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }
    }
}
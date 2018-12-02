﻿using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class RegenerateHealthEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly int hpHealed;

        public RegenerateHealthEvent(GameUnit targetUnit, int hpHealed)
        {
            this.targetUnit = targetUnit;
            this.hpHealed = hpHealed;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            targetUnit.RecoverHP(hpHealed);
            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCellCoordinates(
                targetUnit.Id + " recovers " + hpHealed + " HP!",
                targetUnit.UnitEntity.MapCoordinates,
                50
            );
            AssetManager.SkillBuffSFX.Play();
            Complete = true;
        }
    }
}
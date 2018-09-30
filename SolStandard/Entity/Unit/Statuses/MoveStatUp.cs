﻿using SolStandard.HUD.Window.Content;

namespace SolStandard.Entity.Unit.Statuses
{
    public class MoveStatUp : StatusEffect
    {
        private readonly int mvModifier;

        public MoveStatUp(int turnDuration, int mvModifier) : base(
            statusIcon: new RenderBlank(),
            name: "MV Up!",
            description: "Increased movement distance.",
            turnDuration: turnDuration
        )
        {
            this.mvModifier = mvModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            target.Stats.Mv += mvModifier;
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void RemoveEffect(GameUnit target)
        {
            target.Stats.Mv -= mvModifier;
        }
    }
}
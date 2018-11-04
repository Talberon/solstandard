using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class MoveStatUp : StatusEffect
    {
        private readonly int mvModifier;
        private const int FrameDelay = 10;

        public MoveStatUp(int turnDuration, int mvModifier) : base(
            statusIcon: StatusIconProvider.GetStatusIcon(Utility.Assets.StatusIcon.MvUp, new Vector2(32)),
            name: UnitStatistics.Abbreviation[Stats.Mv] + " Up!",
            description: "Increased movement distance.",
            turnDuration: turnDuration
        )
        {
            this.mvModifier = mvModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            target.Stats.Mv += mvModifier;
            target.UnitEntity.UnitSprite.SetFrameDelay(FrameDelay);

            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }


        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void RemoveEffect(GameUnit target)
        {
            target.Stats.Mv -= mvModifier;
            target.UnitEntity.UnitSprite.ResetFrameDelay();
        }
    }
}
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class MoveStatModifier : StatusEffect
    {
        private readonly int mvModifier;
        private const int FrameDelay = 10;

        public MoveStatModifier(int turnDuration, int mvModifier, string name = null) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Mv, new Vector2(GameDriver.CellSize)),
            name: name ?? UnitStatistics.Abbreviation[Stats.Mv] + (
                      (mvModifier < 0)
                          ? " Down! <" + mvModifier + ">"
                          : " Up! <+" + mvModifier + ">"
                  ),
            description: ((mvModifier < 0) ? "Decreased" : "Increased") + " movement distance.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: (mvModifier < 0)
        )
        {
            this.mvModifier = mvModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.MvModifier += mvModifier;
            target.UnitEntity.UnitSpriteSheet.SetFrameDelay(FrameDelay);

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

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.MvModifier -= mvModifier;
            target.UnitEntity.UnitSpriteSheet.ResetFrameDelay();
        }
    }
}
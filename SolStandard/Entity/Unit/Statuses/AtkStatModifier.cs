using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class AtkStatModifier : StatusEffect
    {
        private readonly int atkModifier;

        public AtkStatModifier(int turnDuration, int atkModifier, string name = null) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Atk, new Vector2(GameDriver.CellSize)),
            name: name ?? UnitStatistics.Abbreviation[Stats.Atk] + (
                      (atkModifier >= 0)
                          ? " Up! <+" + atkModifier + ">"
                          : " Down! <" + atkModifier + ">"
                  ),
            description: ((atkModifier >= 0) ? "In" : "De") + "creased attack power.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: (atkModifier < 0)
        )
        {
            this.atkModifier = atkModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.AtkModifier += atkModifier;

            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.AtkModifier -= atkModifier;
        }
    }
}
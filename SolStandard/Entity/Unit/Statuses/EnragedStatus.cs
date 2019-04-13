using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class EnragedStatus : StatusEffect
    {
        private readonly int atkModifier;

        public EnragedStatus(int turnDuration, int atkModifier) :
            base(
                statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Atk, new Vector2(GameDriver.CellSize)),
                name: "Enraged! <+" + atkModifier + " " + UnitStatistics.Abbreviation[Stats.Atk] + " / " +
                      "-" + atkModifier + " " + UnitStatistics.Abbreviation[Stats.Retribution] + ">",
                description: "Increased attack power / Reduced retribution",
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
            target.Stats.RetModifier -= atkModifier;

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
            target.Stats.RetModifier += atkModifier;
        }
    }
}
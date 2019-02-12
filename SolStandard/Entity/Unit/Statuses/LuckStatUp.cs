using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses
{
    public class LuckStatUp : StatusEffect
    {
        private readonly int luckModifier;

        public LuckStatUp(int turnDuration, int luckModifier) : base(
            statusIcon: UnitStatistics.GetSpriteAtlas(Stats.Luck, new Vector2(GameDriver.CellSize)),
            name: UnitStatistics.Abbreviation[Stats.Luck] + " Up!",
            description: "Increased luck.",
            turnDuration: turnDuration
        )
        {
            this.luckModifier = luckModifier;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.LuckModifier += luckModifier;
            
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                Name,
                50
            );
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            GameContext.GameMapContext.MapContainer.AddNewToastAtUnit(
                target.UnitEntity,
                target.Id + " has status " + Name,
                50
            );
            AssetManager.MapUnitCancelSFX.Play();
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.LuckModifier -= luckModifier;
        }
    }
}
using Microsoft.Xna.Framework;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Creep
{
    public class IndependentStatus : StatusEffect
    {
        public IndependentStatus() : base(
            new SpriteAtlas(AssetManager.IndependentIcon,
                new Vector2(AssetManager.IndependentIcon.Width, AssetManager.IndependentIcon.Height),
                GameDriver.CellSizeVector),
            "Independent",
            "Unit can attack team mates.",
            100,
            false,
            false
        )
        {
        }

        public override void ApplyEffect(GameUnit target)
        {
            //Do nothing
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing
        }

        public override void RemoveEffect(GameUnit target)
        {
            //Do nothing
        }
    }
}
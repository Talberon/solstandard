using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Creep
{
    public class IndependentStatus : StatusEffect
    {
        public IndependentStatus() : base(
            MiscIconProvider.GetMiscIcon(MiscIcon.Independent, GameDriver.CellSizeVector),
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
using SolStandard.Containers.Components.Global;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Statuses.Pugilist
{
    public class FlowStatus : StatusEffect
    {
        public int FlowStacks { get; }

        public FlowStatus(IRenderable icon, int turnDuration, int flowStacks,
            string buffName) : base(
            statusIcon: icon,
            name: buffName + " State <+" + flowStacks + " " + UnitStatistics.Abbreviation[Stats.Atk] + "/+" +
                  flowStacks + " " + UnitStatistics.Abbreviation[Stats.Retribution] + ">",
            description: "Increased attack/counter-attack power.",
            turnDuration: turnDuration,
            hasNotification: false,
            canCleanse: false
        )
        {
            FlowStacks = flowStacks;
        }

        public override void ApplyEffect(GameUnit target)
        {
            AssetManager.SkillBuffSFX.Play();
            target.Stats.AtkModifier += FlowStacks;
            target.Stats.RetModifier += FlowStacks;
            GlobalContext.WorldContext.MapContainer.AddNewToastAtUnit(target.UnitEntity, Name, 50);
        }

        protected override void ExecuteEffect(GameUnit target)
        {
            //Do nothing.
        }

        public override void RemoveEffect(GameUnit target)
        {
            target.Stats.AtkModifier -= FlowStacks;
            target.Stats.RetModifier -= FlowStacks;
        }
    }
}
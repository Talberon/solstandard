using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Events
{
    public class RemoveStatusEffectEvent : IEvent
    {
        private readonly GameUnit targetUnit;
        private readonly StatusEffect effectToRemove;
        public bool Complete { get; private set; }

        public RemoveStatusEffectEvent(GameUnit targetUnit, StatusEffect effectToRemove)
        {
            this.targetUnit = targetUnit;
            this.effectToRemove = effectToRemove;
        }

        public void Continue()
        {
            if (targetUnit.StatusEffects.Contains(effectToRemove))
            {
                AssetManager.SkillBuffSFX.Play();
                effectToRemove.RemoveEffect(targetUnit);
                targetUnit.StatusEffects.Remove(effectToRemove);
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                    "Removed status: " + effectToRemove.Name,
                    50
                );
            }
            else
            {
                AssetManager.WarningSFX.Play();
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                    "Can not remove status unit does not have! <" + effectToRemove.Name + ">",
                    50
                );
            }

            Complete = true;
        }
    }
}
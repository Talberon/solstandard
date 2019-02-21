using SolStandard.Containers.Contexts;
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
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    "Cleansed status: " + effectToRemove.Name + "!",
                    50
                );
            }
            else
            {
                AssetManager.WarningSFX.Play();
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    "Unit can't cleanse status it does not have! <" + effectToRemove.Name + ">",
                    50
                );
            }

            Complete = true;
        }
    }
}
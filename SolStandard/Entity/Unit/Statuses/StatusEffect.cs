using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Statuses
{
    public abstract class StatusEffect
    {
        public IRenderable StatusIcon { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public int TurnDuration { get; private set; }

        protected StatusEffect(int turnDuration)
        {
            TurnDuration = turnDuration;
        }

        public abstract void ApplyEffect(GameUnit target);

        public virtual void UpdateEffect(GameUnit target)
        {
            if (TurnDuration <= 0)
            {
                RemoveEffect(target);
            }

            TurnDuration--;
            ExecuteEffect(target);
        }

        protected abstract void ExecuteEffect(GameUnit target);
        protected abstract void RemoveEffect(GameUnit target);
    }
}
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Statuses
{
    public abstract class StatusEffect
    {
        public IRenderable StatusIcon { get; }
        public string Name { get; protected set; }
        public string Description { get; }
        public int TurnDuration { get; protected set; }
        public bool HasNotification { get; }
        public bool CanCleanse { get; }

        protected StatusEffect(IRenderable statusIcon, string name, string description, int turnDuration,
            bool hasNotification, bool canCleanse)
        {
            StatusIcon = statusIcon;
            Name = name;
            Description = description;
            TurnDuration = turnDuration;
            HasNotification = hasNotification;
            CanCleanse = canCleanse;
        }

        public abstract void ApplyEffect(GameUnit target);

        public void UpdateEffect(GameUnit target)
        {
            TurnDuration--;

            if (TurnDuration < 1)
            {
                ExecuteEffect(target);
                RemoveEffect(target);
                target.StatusEffects.Remove(this);
            }
            else
            {
                ExecuteEffect(target);
            }
        }

        protected abstract void ExecuteEffect(GameUnit target);
        public abstract void RemoveEffect(GameUnit target);
    }
}
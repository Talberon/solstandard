using SolStandard.Containers.Contexts;
using SolStandard.Entity;
using SolStandard.Entity.Unit.Statuses.Duelist;

namespace SolStandard.Utility.Events
{
    public class EndTurnEvent : IEvent
    {
        public bool Complete { get; private set; }
        private readonly bool duelistHasFocusPoints;

        public EndTurnEvent()
        {
            duelistHasFocusPoints = FocusStatus.ActiveDuelistHasFocusPoints;
        }

        public void Continue()
        {
            if (duelistHasFocusPoints)
            {
                (GameContext.ActiveUnit.StatusEffects.Find(status => status is FocusStatus) as FocusStatus)
                    ?.StartAdditionalAction();
            }
            //IMPORTANT Do not allow tiles that have been triggered to trigger again or the risk of soft-locking via infinite triggers can occur
            else if (!GameMapContext.TriggerEffectTiles(EffectTriggerTime.EndOfTurn, false))
            {
                GameMapContext.FinishTurn(false);
            }

            Complete = true;
        }
    }
}
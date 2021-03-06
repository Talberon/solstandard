﻿using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World;
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
                (GlobalContext.ActiveUnit.StatusEffects.Find(status => status is FocusStatus) as FocusStatus)
                    ?.StartAdditionalAction();
            }
            //IMPORTANT Do not allow tiles that have been triggered to trigger again or the risk of soft-locking via infinite triggers can occur
            else if (!WorldContext.TriggerEffectTiles(EffectTriggerTime.EndOfTurn, false))
            {
                WorldContext.FinishTurn(false);
            }

            Complete = true;
        }
    }
}
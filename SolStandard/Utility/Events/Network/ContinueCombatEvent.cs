using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ContinueCombatEvent : NetworkEvent
    {
        public override void Continue()
        {
            GlobalContext.BattleContext.ContinueCombat();
            Complete = true;
        }
    }
}
using System;
using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ContinueCombatEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.BattleContext.ContinueCombat();
            Complete = true;
        }
    }
}
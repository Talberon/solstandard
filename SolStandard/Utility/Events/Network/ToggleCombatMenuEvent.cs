using System;
using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ToggleCombatMenuEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.ToggleCombatMenu();
            Complete = true;
        }
    }
}
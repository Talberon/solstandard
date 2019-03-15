using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class ContinueCombatEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            GameContext.BattleContext.ContinueCombat();
            Complete = true;
        }
    }
}
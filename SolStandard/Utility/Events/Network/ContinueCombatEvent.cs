using SolStandard.Containers.Contexts;

namespace SolStandard.Utility.Events.Network
{
    public class ContinueCombatEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.BattleContext.ContinueCombat();
            Complete = true;
        }
    }
}
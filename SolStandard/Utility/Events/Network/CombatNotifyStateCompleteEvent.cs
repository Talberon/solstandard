using System;
using NLog;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext;
using SolStandard.Containers.Components.World.SubContext.Battle;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class CombatNotifyStateCompleteEvent : NetworkEvent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BattleContext.BattleState battleState;

        public CombatNotifyStateCompleteEvent(BattleContext.BattleState battleState)
        {
            this.battleState = battleState;
        }

        public override void Continue()
        {
            //Notify peer that we have finished the current combat step
            //Don't process this if you sent it yourself
            if (FromServer && GameDriver.ConnectedAsServer)
            {
                //Do nothing
            }
            else if (!FromServer && GameDriver.ConnectedAsClient)
            {
                //Do nothing
            }
            else
            {
                GlobalContext.BattleContext.PeerCanContinue = true;
                Logger.Debug("Received completed battlestate from peer: " + battleState +
                                ". Current state: " + GlobalContext.BattleContext.CurrentState);
            }

            Complete = true;
        }
    }
}
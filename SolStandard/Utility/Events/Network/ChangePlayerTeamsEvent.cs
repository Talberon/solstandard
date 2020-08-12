using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class ChangePlayerTeamsEvent : NetworkEvent
    {
        private readonly Team p1Team;

        public ChangePlayerTeamsEvent(Team p1Team)
        {
            this.p1Team = p1Team;
        }
        
        public override void Continue()
        {
            GlobalContext.SetP1Team(p1Team);
            Complete = true;
        }
    }
}
using System;
using SolStandard.Containers.Contexts;
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
            GameContext.SetP1Team(p1Team);
            Complete = true;
        }
    }
}
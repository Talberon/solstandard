using SolStandard.Entity.Unit;

namespace SolStandard.Utility.Events.Network
{
    public class ChangePlayerTeamsEvent : NetworkEvent
    {
        private readonly Team newP1Team;

        public ChangePlayerTeamsEvent(Team newP1Team)
        {
            this.newP1Team = newP1Team;
        }
        
        public override void Continue()
        {
            GameDriver.SetControllerConfig(newP1Team);
            Complete = true;
        }
    }
}
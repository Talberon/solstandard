using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Utility.Events.Network
{
    public class SpawnUnitEvent : NetworkEvent
    {
        private readonly Role unitRole;
        private readonly Team unitTeam;

        public SpawnUnitEvent(Role unitRole, Team unitTeam)
        {
            this.unitRole = unitRole;
            this.unitTeam = unitTeam;
        }

        public override void Continue()
        {
            SpawnUnitAction.PlaceUnitInTile(unitRole, unitTeam);
            Complete = true;
        }
    }
}
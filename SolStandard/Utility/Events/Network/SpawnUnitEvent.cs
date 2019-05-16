using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Utility.Events.Network
{
    public class SpawnUnitEvent : NetworkEvent
    {
        private readonly Role unitRole;
        private readonly Team unitTeam;
        private readonly Vector2 coordinates;

        public SpawnUnitEvent(Role unitRole, Team unitTeam, Vector2 coordinates)
        {
            this.unitRole = unitRole;
            this.unitTeam = unitTeam;
            this.coordinates = coordinates;
        }

        public override void Continue()
        {
            SpawnUnitAction.PlaceUnitInTile(unitRole, unitTeam, coordinates);
            Complete = true;
        }
    }
}
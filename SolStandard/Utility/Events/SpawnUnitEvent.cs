using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Utility.Events
{
    public class SpawnUnitEvent : IEvent
    {
        private readonly Role unitRole;
        private readonly Team unitTeam;
        private readonly float x;
        private readonly float y;
        public bool Complete { get; private set; }

        public SpawnUnitEvent(Role unitRole, Team unitTeam, Vector2 coordinates)
        {
            this.unitRole = unitRole;
            this.unitTeam = unitTeam;
            x = coordinates.X;
            y = coordinates.Y;
        }


        public void Continue()
        {
            SpawnUnitAction.PlaceUnitInTile(unitRole, unitTeam, new Vector2(x, y));
            Complete = true;
        }
    }
}
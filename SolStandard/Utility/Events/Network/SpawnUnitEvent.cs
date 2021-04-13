using System;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class SpawnUnitEvent : NetworkEvent
    {
        private readonly Role unitRole;
        private readonly Team unitTeam;
        private readonly float x;
        private readonly float y;

        public SpawnUnitEvent(Role unitRole, Team unitTeam, Vector2 coordinates)
        {
            this.unitRole = unitRole;
            this.unitTeam = unitTeam;
            x = coordinates.X;
            y = coordinates.Y;
        }


        public override void Continue()
        {
            SpawnUnitAction.PlaceUnitInTile(unitRole, unitTeam, new Vector2(x, y));
            Complete = true;
        }
    }
}
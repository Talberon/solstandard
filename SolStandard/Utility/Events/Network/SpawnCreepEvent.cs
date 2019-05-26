using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Creeps;

namespace SolStandard.Utility.Events.Network
{
    public class SpawnCreepEvent : NetworkEvent
    {
        private readonly Role unitRole;
        private readonly Dictionary<string, string> entityProperties;
        private readonly Vector2 coordinates;

        public SpawnCreepEvent(Role unitRole, Vector2 coordinates, Dictionary<string, string> entityProperties)
        {
            this.unitRole = unitRole;
            this.coordinates = coordinates;
            this.entityProperties = entityProperties;
        }

        public override void Continue()
        {
            SummoningRoutine.PlaceCreepInTile(unitRole, coordinates, entityProperties);
            Complete = true;
        }
    }
}
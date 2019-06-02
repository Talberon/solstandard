using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Creeps;

namespace SolStandard.Utility.Events.AI
{
    public class SpawnCreepEvent : IEvent
    {
        private readonly Role unitRole;
        private readonly Dictionary<string, string> entityProperties;
        private readonly Vector2 coordinates;
        public bool Complete { get; private set; }

        public SpawnCreepEvent(Role unitRole, Vector2 coordinates, Dictionary<string, string> entityProperties)
        {
            this.unitRole = unitRole;
            this.entityProperties = entityProperties;
            this.coordinates = coordinates;
        }

        public void Continue()
        {
            SummoningRoutine.PlaceCreepInTile(unitRole, coordinates, entityProperties);
            Complete = true;
        }
    }
}
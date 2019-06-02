using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions.Creeps;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class SpawnCreepEvent : NetworkEvent
    {
        private readonly Role unitRole;
        private readonly Dictionary<string, string> entityProperties;
        private readonly float x;
        private readonly float y;

        public SpawnCreepEvent(Role unitRole, Vector2 coordinates, Dictionary<string, string> entityProperties)
        {
            this.unitRole = unitRole;
            this.entityProperties = entityProperties;
            x = coordinates.X;
            y = coordinates.Y;
        }

        public override void Continue()
        {
            SummoningRoutine.PlaceCreepInTile(unitRole, new Vector2(x, y), entityProperties);
            Complete = true;
        }
    }
}
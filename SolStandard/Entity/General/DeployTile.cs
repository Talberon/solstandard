using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class DeployTile : TerrainEntity
    {
        public Team DeployTeam { get; private set; }
        public bool Occupied { get; set; }

        public DeployTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates, Team deployTeam,
            Dictionary<string, string> tiledProperties)
            : base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            DeployTeam = deployTeam;
            Occupied = false;
        }
    }
}
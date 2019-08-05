using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class DeployTile : TerrainEntity
    {
        public Team DeployTeam { get; }
        public bool Occupied { get; }

        public DeployTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates, Team deployTeam)
            : base(name, type, sprite, mapCoordinates)
        {
            DeployTeam = deployTeam;
            Occupied = false;
        }
    }
}
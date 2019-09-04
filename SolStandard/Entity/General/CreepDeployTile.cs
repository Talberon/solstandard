using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class CreepDeployTile : TerrainEntity
    {
        public string CreepPool { get; }
        public bool CopyCreep { get; }

        public CreepDeployTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string creepPool,
            bool copyCreep)
            : base(name, type, sprite, mapCoordinates)
        {
            CreepPool = creepPool;
            CopyCreep = copyCreep;
        }
    }
}
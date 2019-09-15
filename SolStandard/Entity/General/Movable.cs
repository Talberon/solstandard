using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Movable : TerrainEntity
    {
        public Movable(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool canMove) :
            base(name, type, sprite, mapCoordinates)
        {
            CanMove = canMove;
        }
    }
}
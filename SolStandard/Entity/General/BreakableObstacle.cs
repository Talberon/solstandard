using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class BreakableObstacle : MapEntity
    {
        private readonly int hp;
        private bool canMove;
        private bool isBroken;

        public BreakableObstacle(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int hp, bool canMove, bool isBroken) : base(name, type, sprite,
            mapCoordinates, tiledProperties)
        {
            this.hp = hp;
            this.canMove = canMove;
            this.isBroken = isBroken;
        }
    }
}
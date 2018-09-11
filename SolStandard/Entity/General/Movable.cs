using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Movable : MapEntity
    {
        private readonly bool canMove;

        public Movable(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool canMove) : base(name, type, sprite, mapCoordinates,
            tiledProperties)
        {
            this.canMove = canMove;
        }
    }
}
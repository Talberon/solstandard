using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Barricade : BreakableObstacle, IItem
    {
        public Barricade(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int hp) : 
            base(name, type, sprite, mapCoordinates, new Dictionary<string, string>(), hp, false, hp < 1, 0)
        {
        }

        public IRenderable Icon
        {
            get { return Sprite; }
        }

        public UnitAction UseAction()
        {
            return new DeployBarricadeAction(this);
        }

        public UnitAction DropAction()
        {
            return new Wait();
        }
    }
}
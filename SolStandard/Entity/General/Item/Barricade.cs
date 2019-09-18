using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Utility;

namespace SolStandard.Entity.General.Item
{
    public class Barricade : BreakableObstacle, IItem
    {
        public string ItemPool { get; }

        public Barricade(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int hp,
            string itemPool) :
            base(name, type, sprite, mapCoordinates, hp, false, hp < 1, 0)
        {
            ItemPool = itemPool;
        }

        public IRenderable Icon => Sprite;


        public UnitAction UseAction()
        {
            return new DeployBarricadeAction(this);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Barricade(Name, Type, Sprite, MapCoordinates, HP, ItemPool);
        }
    }
}
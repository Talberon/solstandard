using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Utility;

namespace SolStandard.Entity.General.Item
{
    public class LadderBridge : Movable, IItem
    {
        public IRenderable Icon { get; }
        public string ItemPool { get; }

        public LadderBridge(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string itemPool,
            bool canMove) :
            base(name, type, sprite, mapCoordinates, canMove)
        {
            Icon = sprite;
            ItemPool = itemPool;
        }

        public bool IsBroken => false;

        public UnitAction UseAction()
        {
            return new DeployLadderBridgeAction(this);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new LadderBridge(Name, Type, Sprite, MapCoordinates, ItemPool, CanMove);
        }
    }
}
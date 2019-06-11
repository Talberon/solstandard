using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.Utility;

namespace SolStandard.Entity.General.Item
{
    public class LadderBridge : Movable, IItem
    {
        public IRenderable Icon { get; private set; }
        public string ItemPool { get; private set; }

        public LadderBridge(string name, string type, IRenderable sprite, Vector2 mapCoordinates, string itemPool,
            bool canMove, Dictionary<string, string> tiledProperties) :
            base(name, type, sprite, mapCoordinates, tiledProperties, canMove)
        {
            Icon = sprite;
            ItemPool = itemPool;
        }

        public bool IsBroken
        {
            get { return false; }
        }

        public UnitAction UseAction()
        {
            return new DeployLadderBridgeAction(this);
        }

        public UnitAction DropAction()
        {
            return new TradeItemAction(this);
        }

        public IItem Duplicate()
        {
            return new LadderBridge(Name, Type, Sprite, MapCoordinates, ItemPool, CanMove, TiledProperties);
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Key : TerrainEntity, IItem, IActionTile
    {
        public string UsedWith { get; }
        public bool IsMasterKey { get; }
        public int[] InteractRange { get; }
        public string ItemPool { get; }

        public Key(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            string usedWith, int[] range, string itemPool, bool isMasterKey) :
            base(name, type, sprite, mapCoordinates)
        {
            UsedWith = usedWith;
            InteractRange = range;
            ItemPool = itemPool;
            IsMasterKey = isMasterKey;
        }

        public bool IsBroken => false;

        public IRenderable Icon => Sprite;

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpItemAction(this, MapCoordinates)
            };
        }

        public UnitAction UseAction()
        {
            return new ToggleLockAction(this);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Key(Name, Type, Sprite, MapCoordinates, UsedWith, InteractRange, ItemPool, IsMasterKey);
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        new SpriteAtlas(
                            AssetManager.LockTexture,
                            new Vector2(AssetManager.LockTexture.Width),
                            GameDriver.CellSizeVector,
                            Convert.ToInt32(Chest.LockIconState.Unlocked)
                        ),
                        new RenderText(AssetManager.WindowFont, ": " + UsedWith)
                    }
                }
            );
    }
}
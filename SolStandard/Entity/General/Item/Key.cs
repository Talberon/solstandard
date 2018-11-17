using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Key : TerrainEntity, IItem, IActionTile
    {
        public string UsedWith { get; private set; }
        public int[] Range { get; private set; }

        public Key(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string usedWith, int[] range) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            UsedWith = usedWith;
            Range = range;
        }

        public bool IsBroken
        {
            get { return false; }
        }

        public IRenderable Icon
        {
            get { return Sprite; }
        }

        public UnitAction TileAction()
        {
            return new PickUpItemAction(this, MapCoordinates);
        }

        public UnitAction UseAction()
        {
            return new ToggleLockAction(this);
        }

        public UnitAction DropAction()
        {
            return new DropItemAction(this);
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            InfoHeader,
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "Used with: " + UsedWith),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}
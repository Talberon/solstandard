using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Weapon : TerrainEntity, IItem, IActionTile
    {
        public int[] Range { get; private set; }
        private WeaponStatistics WeaponStatistics { get; set; }

        public Weapon(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int atkValue, int luckModifier, int[] atkRange)
            : base(name, type, sprite, mapCoordinates, new Dictionary<string, string>())
        {
            Range = pickupRange;
            WeaponStatistics = new WeaponStatistics(atkValue, luckModifier, atkRange);
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
            return new WeaponAttack(Sprite, Name, WeaponStatistics);
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
                            new RenderText(AssetManager.WindowFont, "Stats: " + Environment.NewLine + WeaponStatistics),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}
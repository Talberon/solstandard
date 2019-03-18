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
        public int[] InteractRange { get; private set; }
        private WeaponStatistics WeaponStatistics { get; set; }

        public Weapon(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int atkValue, int luckModifier, int[] atkRange, int usesRemaining)
            : base(name, type, sprite, mapCoordinates, new Dictionary<string, string>())
        {
            InteractRange = pickupRange;
            WeaponStatistics = new WeaponStatistics(atkValue, luckModifier, atkRange, usesRemaining);
        }

        public bool IsBroken
        {
            get { return WeaponStatistics.IsBroken; }
        }

        public IRenderable Icon
        {
            get { return Sprite; }
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new PickUpItemAction(this, MapCoordinates)
            };
        }

        public UnitAction UseAction()
        {
            return new WeaponAttack(Sprite, Name, WeaponStatistics);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Weapon(Name, Type, Sprite, MapCoordinates, InteractRange, WeaponStatistics.AtkValue,
                WeaponStatistics.LuckModifier, WeaponStatistics.AtkRange, WeaponStatistics.UsesRemaining);
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
                            new RenderText(
                                AssetManager.WindowFont,
                                "Pick-Up Range: " + string.Format("[{0}]", string.Join(",", InteractRange))
                            ),
                            new RenderBlank()
                        },
                        {
                            new RenderText(AssetManager.WindowFont, WeaponStatistics.ToString()),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}
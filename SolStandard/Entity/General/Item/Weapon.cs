using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Item;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General.Item
{
    public class Weapon : TerrainEntity, IItem, IActionTile
    {
        public int[] InteractRange { get; }
        private WeaponStatistics WeaponStatistics { get; }
        private readonly Window statWindow;
        public string ItemPool { get; }

        public Weapon(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int[] pickupRange,
            int atkValue, int luckModifier, int[] atkRange, int usesRemaining, string itemPool)
            : base(name, type, sprite, mapCoordinates)
        {
            InteractRange = pickupRange;
            ItemPool = itemPool;
            WeaponStatistics = new WeaponStatistics(atkValue, luckModifier, atkRange, usesRemaining);
            statWindow = BuildStatWindow(WeaponStatistics);
        }

        public static Window BuildStatWindow(WeaponStatistics weaponStatistics)
        {
            IRenderable[,] statWindowGrid =
            {
                {new RenderText(AssetManager.WindowFont, "-Stats-")},
                {weaponStatistics.GenerateStatGrid(AssetManager.WindowFont)}
            };

            return new Window(new WindowContentGrid(statWindowGrid, 2, HorizontalAlignment.Centered), InnerWindowColor);
        }

        public bool IsBroken => WeaponStatistics.IsBroken;

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
            return new WeaponAttack(Sprite, WeaponStatistics, this);
        }

        public UnitAction DropAction()
        {
            return new DropGiveItemAction(this);
        }

        public IItem Duplicate()
        {
            return new Weapon(Name, Type, Sprite, MapCoordinates, InteractRange, WeaponStatistics.AtkValue,
                WeaponStatistics.LuckModifier, WeaponStatistics.AtkRange, WeaponStatistics.UsesRemaining, ItemPool);
        }

        protected override IRenderable EntityInfo => statWindow;
    }
}
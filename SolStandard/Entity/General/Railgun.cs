using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Railgun : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; }
        private int AtkRange { get; }
        private readonly WeaponStatistics weaponStatistics;
        private readonly Window statWindow;

        public Railgun(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool canMove, int atkRange,
            int atkDamage) :
            base(name, type, sprite, mapCoordinates)
        {
            CanMove = canMove;
            AtkRange = atkRange;
            InteractRange = new[] {0};
            weaponStatistics = new WeaponStatistics(atkDamage, -100, new[] {AtkRange}, 100);
            statWindow = Weapon.BuildStatWindow(weaponStatistics);
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new RailgunAction(Sprite, AtkRange, weaponStatistics)
            };
        }

        public override IRenderable TerrainInfo =>
            new WindowContentGrid(
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
                        statWindow,
                        new RenderBlank()
                    }
                },
                1
            );
    }
}
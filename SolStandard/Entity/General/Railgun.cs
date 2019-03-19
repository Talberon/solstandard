using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Railgun : TerrainEntity, IActionTile
    {
        public int[] InteractRange { get; private set; }
        public int AtkRange { get; private set; }
        private readonly int atkDamage;

        public Railgun(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool canMove, int atkRange, int atkDamage) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            CanMove = canMove;
            AtkRange = atkRange;
            this.atkDamage = atkDamage;
            InteractRange = new[] {0};
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new RailgunAction(Sprite, AtkRange, atkDamage)
            };
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
                            UnitStatistics.GetSpriteAtlas(Stats.AtkRange),
                            new RenderText(AssetManager.WindowFont, "Range: " + AtkRange)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        }
                    },
                    1
                );
            }
        }
    }
}
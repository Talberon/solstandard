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
        public int[] Range { get; private set; }
        public int AtkRange { get; private set; }

        public Railgun(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool canMove, int atkRange) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            CanMove = canMove;
            AtkRange = atkRange;
            Range = new[] {0};
        }

        public UnitAction TileAction()
        {
            return new RailgunAction(Sprite, AtkRange);
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
                            new RenderText(AssetManager.WindowFont, "Range:" + AtkRange)
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
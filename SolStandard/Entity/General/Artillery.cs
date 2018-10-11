using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;
using SolStandard.Entity.Unit.Skills.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Artillery : TerrainEntity, IActionTile
    {
        public int[] Range { get; private set; }
        public int[] AtkRange { get; private set; }

        public Artillery(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool canMove, int[] atkRange) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            CanMove = canMove;
            AtkRange = atkRange;
            Range = new[] {0};
        }

        public UnitAction TileAction()
        {
            return new ArtilleryAction(Sprite, AtkRange);
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
                            UnitStatistics.GetSpriteAtlas(StatIcons.AtkRange),
                            new RenderText(AssetManager.WindowFont,
                                "Range:" + string.Format("[{0}]", string.Join(",", AtkRange))
                            )
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
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
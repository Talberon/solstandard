using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class BuffTile : TerrainEntity
    {
        private readonly int modifier;
        private readonly StatIcons buffStat;
        private readonly bool canMove;

        public BuffTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int modifier, StatIcons buffStat, bool canMove) : base(name,
            type, sprite, mapCoordinates, tiledProperties)
        {
            this.modifier = modifier;
            this.buffStat = buffStat;
            this.canMove = canMove;
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
                            UnitStatistics.GetSpriteAtlas(buffStat),
                            new RenderText(AssetManager.WindowFont, buffStat.ToString().ToUpper() + ": +" + modifier),
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, (canMove) ? "Can Move" : "No Move",
                                (canMove) ? PositiveColor : NegativeColor)
                        }
                    },
                    3
                );
            }
        }
    }
}
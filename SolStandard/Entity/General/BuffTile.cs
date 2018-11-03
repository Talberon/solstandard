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
        public Stats BuffStat { get; private set; }
        public int Modifier { get; private set; }
        private readonly bool canMove;

        private static readonly Dictionary<string, Stats> BonusStatDictionary =
            new Dictionary<string, Stats>
            {
                {"ATK", Stats.Atk},
                {"DEF", Stats.Armor}
            };

        public BuffTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int modifier, string buffStat, bool canMove) : base(name,
            type, sprite, mapCoordinates, tiledProperties)
        {
            Modifier = modifier;
            BuffStat = BonusStatDictionary[buffStat];
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
                            UnitStatistics.GetSpriteAtlas(BuffStat),
                            new RenderText(AssetManager.WindowFont, BuffStat.ToString().ToUpper() + ": +" + Modifier),
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
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
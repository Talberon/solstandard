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
        private static readonly Dictionary<StatIcons, StatIcons> StatToBonusStatDictionary =
            new Dictionary<StatIcons, StatIcons>
            {
                {StatIcons.Hp, StatIcons.BonusHp},
                {StatIcons.Atk, StatIcons.BonusAtk},
                {StatIcons.Def, StatIcons.BonusDef},
                {StatIcons.Sp, StatIcons.BonusSp},
                {StatIcons.Mv, StatIcons.BonusMv},
                {StatIcons.AtkRange, StatIcons.BonusAtkRange}
            };

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
                            UnitStatistics.GetSpriteAtlas(StatToBonusStatDictionary[buffStat]),
                            new RenderText(AssetManager.WindowFont, buffStat.ToString().ToUpper() + ": +" + modifier.ToString()),
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
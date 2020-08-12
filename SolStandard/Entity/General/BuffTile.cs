using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class BuffTile : TerrainEntity
    {
        public BonusStatistics BonusStatistics { get; }


        public BuffTile(string name, string type, IRenderable sprite, Vector2 mapCoordinates, int atkBonus,
            int retBonus, int blockBonus, int luckBonus) : base(name, type, sprite, mapCoordinates)
        {
            BonusStatistics = new BonusStatistics(atkBonus, retBonus, blockBonus, luckBonus);
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(new IRenderable[,]
                {
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Atk),
                        new RenderText(AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Atk] + ": +" + BonusStatistics.AtkBonus)
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Retribution),
                        new RenderText(AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Retribution] + ": +" +
                            BonusStatistics.RetBonus)
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Block),
                        new RenderText(AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Block] + ": +" +
                            BonusStatistics.BlockBonus)
                    },
                    {
                        UnitStatistics.GetSpriteAtlas(Stats.Luck),
                        new RenderText(AssetManager.WindowFont,
                            UnitStatistics.Abbreviation[Stats.Luck] + ": +" +
                            BonusStatistics.LuckBonus)
                    }
                },
                1,
                HorizontalAlignment.Centered
            );
    }
}
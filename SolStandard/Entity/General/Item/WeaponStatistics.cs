using System;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.General.Item
{
    public class WeaponStatistics
    {
        public int UsesRemaining { get; private set; }
        public int AtkValue { get; }
        public int LuckModifier { get; }
        public int[] AtkRange { get; }

        public WeaponStatistics(int atkValue, int luckModifier, int[] atkRange, int usesRemaining)
        {
            AtkValue = atkValue;
            LuckModifier = luckModifier;
            AtkRange = atkRange;
            UsesRemaining = usesRemaining;
        }

        public bool IsBroken => UsesRemaining < 1;

        public void DecrementRemainingUses()
        {
            UsesRemaining--;
        }

        public override string ToString()
        {
            return string.Format("Stats: " + Environment.NewLine +
                                 "{0}: [{1}]" + Environment.NewLine +
                                 "{2}: [{3}]" + Environment.NewLine +
                                 "{4}: [{5}]" + Environment.NewLine +
                                 "{6}: [{7}]",
                "Uses Remaining", UsesRemaining,
                UnitStatistics.Abbreviation[Stats.Atk], AtkValue,
                UnitStatistics.Abbreviation[Stats.Luck], ((LuckModifier > 0) ? "+" : string.Empty) + LuckModifier,
                UnitStatistics.Abbreviation[Stats.AtkRange], string.Join(",", AtkRange)
            );
        }

        public WindowContentGrid GenerateStatGrid(ISpriteFont textFont)
        {
            IRenderable[,] statGrid =
            {
                {
                    new WindowContentGrid(new IRenderable[,]
                    {
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Atk, GameDriver.CellSizeVector),
                            new RenderText(textFont, UnitStatistics.Abbreviation[Stats.Atk] + ": " + AtkValue)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.AtkRange, GameDriver.CellSizeVector),
                            new RenderText(
                                textFont,
                                UnitStatistics.Abbreviation[Stats.AtkRange] + ": [" + string.Join(",", AtkRange) + "]"
                            )
                        }
                    }, 0),
                    new WindowContentGrid(new IRenderable[,]
                    {
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Luck, GameDriver.CellSizeVector),
                            new RenderText(
                                textFont,
                                UnitStatistics.Abbreviation[Stats.Luck] + ": " +
                                ((LuckModifier > 0) ? "+" : string.Empty) + LuckModifier
                            )
                        },
                        {
                            StatusIconProvider.GetStatusIcon(StatusIcon.Durability, GameDriver.CellSizeVector),
                            new RenderText(textFont, "Uses : [" + UsesRemaining + "]")
                        }
                    }, 0)
                }
            };

            return new WindowContentGrid(statGrid, 3);
        }
    }
}
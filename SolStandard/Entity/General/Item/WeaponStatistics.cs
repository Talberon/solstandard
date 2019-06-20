using System;
using Microsoft.Xna.Framework;
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
        public int AtkValue { get; private set; }
        public int LuckModifier { get; private set; }
        public int[] AtkRange { get; private set; }

        public WeaponStatistics(int atkValue, int luckModifier, int[] atkRange, int usesRemaining)
        {
            AtkValue = atkValue;
            LuckModifier = luckModifier;
            AtkRange = atkRange;
            UsesRemaining = usesRemaining;
        }

        public bool IsBroken
        {
            get { return UsesRemaining < 1; }
        }

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
                    UnitStatistics.GetSpriteAtlas(Stats.Atk, new Vector2(GameDriver.CellSize)),
                    new RenderText(textFont, UnitStatistics.Abbreviation[Stats.Atk] + ": " + AtkValue)
                },
                {
                    UnitStatistics.GetSpriteAtlas(Stats.Luck, new Vector2(GameDriver.CellSize)),
                    new RenderText(textFont,
                        UnitStatistics.Abbreviation[Stats.Luck] + ": " + ((LuckModifier > 0) ? "+" : string.Empty) +
                        LuckModifier)
                },
                {
                    UnitStatistics.GetSpriteAtlas(Stats.AtkRange, new Vector2(GameDriver.CellSize)),
                    new RenderText(textFont,
                        UnitStatistics.Abbreviation[Stats.AtkRange] + ": [" + string.Join(",", AtkRange) + "]")
                },
                {
                    StatusIconProvider.GetStatusIcon(StatusIcon.Durability, new Vector2(GameDriver.CellSize)),
                    new RenderText(textFont, "Uses : [" + UsesRemaining + "]")
                },
            };

            return new WindowContentGrid(statGrid, 3);
        }
    }
}
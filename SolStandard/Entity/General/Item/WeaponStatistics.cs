using System;
using SolStandard.Entity.Unit;

namespace SolStandard.Entity.General.Item
{
    public class WeaponStatistics
    {
        public int AtkValue { get; private set; }
        public int LuckModifier { get; private set; }
        public int[] AtkRange { get; private set; }

        public WeaponStatistics(int atkValue, int luckModifier, int[] atkRange)
        {
            AtkValue = atkValue;
            LuckModifier = luckModifier;
            AtkRange = atkRange;
        }

        public override string ToString()
        {
            return string.Format("Stats: " + Environment.NewLine +
                                 "[{0}: {1}]" + Environment.NewLine +
                                 "[{2}: {3}]" + Environment.NewLine +
                                 "[{4}: {5}]",
                UnitStatistics.Abbreviation[Stats.Atk], AtkValue,
                UnitStatistics.Abbreviation[Stats.Luck], ((LuckModifier > 0) ? "+" : string.Empty) + LuckModifier,
                UnitStatistics.Abbreviation[Stats.AtkRange], string.Join(",", AtkRange)
            );
        }
    }
}
namespace SolStandard.Containers.Components.Global
{
    public readonly struct BonusStatistics
    {
        public readonly int AtkBonus;
        public readonly int RetBonus;
        public readonly int BlockBonus;
        public readonly int LuckBonus;

        public BonusStatistics(int atkBonus, int retBonus, int blockBonus, int luckBonus)
        {
            AtkBonus = atkBonus;
            RetBonus = retBonus;
            BlockBonus = blockBonus;
            LuckBonus = luckBonus;
        }

        public static BonusStatistics operator +(BonusStatistics stat1, BonusStatistics stat2)
        {
            return new BonusStatistics(
                stat1.AtkBonus + stat2.AtkBonus,
                stat1.RetBonus + stat2.RetBonus,
                stat1.BlockBonus + stat2.BlockBonus,
                stat1.LuckBonus + stat2.LuckBonus
            );
        }
    }
}
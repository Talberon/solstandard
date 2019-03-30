namespace SolStandard.Containers.Contexts.Combat
{
    public struct TerrainBonus
    {
        public readonly int AtkBonus;
        public readonly int RetBonus;
        public readonly int BlockBonus;
        public readonly int LuckBonus;

        public TerrainBonus(int atkBonus, int retBonus, int blockBonus, int luckBonus)
        {
            AtkBonus = atkBonus;
            RetBonus = retBonus;
            BlockBonus = blockBonus;
            LuckBonus = luckBonus;
        }
    }
}
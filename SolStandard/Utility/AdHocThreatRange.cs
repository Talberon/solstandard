using SolStandard.Entity;

namespace SolStandard.Utility
{
    public class AdHocThreatRange : IThreatRange
    {
        public int[] AtkRange { get; }
        public int MvRange { get; }

        public AdHocThreatRange(int[] atkRange, int mvRange)
        {
            AtkRange = atkRange;
            MvRange = mvRange;
        }
    }
}
using SolStandard.Entity;

namespace SolStandard.Utility
{
    public class AdHocThreatRange : IThreatRange
    {
        public int[] AtkRange { get; private set; }
        public int MvRange { get; private set; }

        public AdHocThreatRange(int[] atkRange, int mvRange)
        {
            AtkRange = atkRange;
            MvRange = mvRange;
        }
    }
}